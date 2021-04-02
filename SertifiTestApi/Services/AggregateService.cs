using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SertifiTestApi.Clients;
using SertifiTestApi.Models;
using SertifiTestApi.Services.Extensions;

namespace SertifiTestApi.Services
{
    public class AggregateService : IAggregateService
    {
        private readonly ISertifiHttpClient _sertifiService;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AggregateService> _logger;
        private const string StudentRecordsCacheKey = "StudentRecords";

        public AggregateService(ISertifiHttpClient sertifiService, IMemoryCache memoryCache, ILogger<AggregateService> logger)
        {
            _sertifiService = sertifiService;
            _cache = memoryCache;
            _logger = logger;
        }

        private MemoryCacheEntryOptions CacheOptions { get; } = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };

        public async Task<StudentsAggregate> GetAggregate()
        {
            var studentRecords = await GetStudentRecords();

            studentRecords.ToList().Validate();

            var dataByYear = studentRecords.GetDataByYear();

            var aggregate = new StudentsAggregate();

            Parallel.Invoke(
                () => aggregate.YearWithHighestAttendance = GetYearWithHighestAttendance(dataByYear),
                () => aggregate.YearWithHighestAverageGpa = GetYearWithHighestAverageGpa(dataByYear),
                () => aggregate.Top10StudentIdsWithHighestGpa = GetTop10StudentIdsWithHighestGpa(studentRecords),
                () => aggregate.StudentIdMostInconsistent = GetStudentIdMostInconsistent(studentRecords)
            );

            return aggregate;
        }

        public async Task<bool> SubmitAggregate(StudentsAggregateSubmitRequest request)
        {
            return await _sertifiService.SubmitAggregate(request);
        }

        private Task<IReadOnlyList<StudentRecordResponse>> GetStudentRecords() => _cache.GetOrCreateAsync(StudentRecordsCacheKey, async entry =>
        {
            entry.SetOptions(CacheOptions);

            var studentRecords = await _sertifiService.GetStudentRecords();

            _logger.LogInformation($"Local cache for {StudentRecordsCacheKey} updated.");

            return studentRecords;
        });

        private List<int> GetTop10StudentIdsWithHighestGpa(IEnumerable<StudentRecordResponse> records)
        {
            return records.Select(x => new {x.Id, Sum = x.GPARecord.Sum()})
                .OrderByDescending(x => x.Sum)
                .Take(10)
                .Select(x => x.Id)
                .ToList();
        }

        private int GetStudentIdMostInconsistent(IEnumerable<StudentRecordResponse> records)
        {
            return records.Select(x => new { x.Id, Diff = x.GPARecord.Max() - x.GPARecord.Min() })
                .OrderByDescending(x => x.Diff)
                .Select(x => x.Id)
                .First();
        }

        private int GetYearWithHighestAverageGpa(Dictionary<int, (double overallGpa, int numberOfOccurrences)> dataByYear)
        {
            return dataByYear
                .OrderByDescending(x => x.Value.overallGpa / x.Value.numberOfOccurrences)
                .FirstOrDefault()
                .Key;
        }

        private int GetYearWithHighestAttendance(Dictionary<int, (double overallGpa, int numberOfOccurrences)> dataByYear)
        {
            return dataByYear
                .OrderByDescending(x => x.Value.numberOfOccurrences)
                .ThenBy(x => x.Key)
                .FirstOrDefault()
                .Key;
        }
    }
}
