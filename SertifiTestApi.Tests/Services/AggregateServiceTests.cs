using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SertifiTestApi.Clients;
using SertifiTestApi.Models;
using SertifiTestApi.Services;
using SertifiTestApi.Tests.Utils;
using Xunit;

namespace SertifiTestApi.Tests.Services
{
    public class AggregateServiceTests
    {
        private readonly Mock<ISertifiHttpClient> _sertifiHttpClient;
        private Mock<ILogger<AggregateService>> _logger;

        public AggregateServiceTests()
        {
            _sertifiHttpClient = new Mock<ISertifiHttpClient>();
            _logger = new Mock<ILogger<AggregateService>>();
        }

        [Theory]
        // original data from http://apitest.sertifi.net/api/Students
        [InlineData("StudentsData.json", 2011, 2016, 15, new int[] { 18, 4, 12, 11, 20, 13, 24, 6, 22, 21 })]
        public async Task GetAggregate_ShouldReturnCorrectData(string fileName, 
            int yearWithHighestAttendance,
            int yearWithHighestAverageGpa,
            int studentIdMostInconsistent,
            int[] top10StudentIdsWithHighestGpa)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var studentRecords = JsonUtils.DeserializeByPath<List<StudentRecordResponse>>(fileName);

            _sertifiHttpClient.Setup(x => x.GetStudentRecords())
                .ReturnsAsync(studentRecords);

            var service = new AggregateService(_sertifiHttpClient.Object , cache, _logger.Object);

            var aggregate = await service.GetAggregate();
            await service.GetAggregate();

            Assert.Equal(yearWithHighestAttendance, aggregate.YearWithHighestAttendance);
            Assert.Equal(yearWithHighestAverageGpa, aggregate.YearWithHighestAverageGpa);
            Assert.Equal(studentIdMostInconsistent, aggregate.StudentIdMostInconsistent);
            Assert.Equal(top10StudentIdsWithHighestGpa, aggregate.Top10StudentIdsWithHighestGpa);

            _sertifiHttpClient.Verify(x => x.GetStudentRecords(), Times.Once);
        }

        [Fact]
        public async Task GetAggregate_ShouldCacheData()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            _sertifiHttpClient.Setup(x => x.GetStudentRecords())
                .ReturnsAsync(new List<StudentRecordResponse>
                {
                    new StudentRecordResponse
                    {
                        Id = 1, Name = "Jack", StartYear = 2013, EndYear = 2013, GPARecord = new List<double> {3.4}
                    }
                });

            var service = new AggregateService(_sertifiHttpClient.Object, cache, _logger.Object);

            await service.GetAggregate();
            await service.GetAggregate();
            await service.GetAggregate();

            _sertifiHttpClient.Verify(x => x.GetStudentRecords(), Times.Once);
        }

        [Theory]
        [InlineData("IncorrectGPACountData.json", "Number of GPAs must match the length of period.")]
        [InlineData("IncorrectPeriodData.json", "EndDate should not be earlier than StartDate.")]
        public async Task GetAggregate_ShouldValidateData(string fileName, string expectedMessage)
        {
            var cache = new MemoryCache(new MemoryCacheOptions());

            var studentRecords = JsonUtils.DeserializeByPath<List<StudentRecordResponse>>(fileName);

            _sertifiHttpClient.Setup(x => x.GetStudentRecords())
                .ReturnsAsync(studentRecords);

            var service = new AggregateService(_sertifiHttpClient.Object, cache, _logger.Object);

            var exception = Assert.ThrowsAsync<InvalidDataException>(() => service.GetAggregate());
            Assert.Equal(expectedMessage, exception.Result.Message);
        }
    }
}
