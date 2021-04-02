using System.Collections.Generic;
using System.IO;
using System.Linq;
using SertifiTestApi.Models;

namespace SertifiTestApi.Services.Extensions
{
    public static class StudentRecordExtensions
    {
        public static void Validate(this List<StudentRecordResponse> records)
        {
            if (records.Any(x => x.EndYear < x.StartYear))
            {
                throw new InvalidDataException("EndDate should not be earlier than StartDate.");
            }

            if (records.ToList().Any(x => x.EndYear - x.StartYear != x.GPARecord.Count - 1))
            {
                throw new InvalidDataException("Number of GPAs must match the length of period.");
            }
        }

        public static Dictionary<int, (double overallGpa, int numberOfOccurrences)> GetDataByYear(this IEnumerable<StudentRecordResponse> records)
        {
            var dict = new Dictionary<int, (double overallGpa, int numberOfOccurrences)>();

            foreach (var r in records)
            {
                for (var year = r.StartYear; year <= r.EndYear; year++)
                {
                    var gpa = r.GPARecord[year - r.StartYear];

                    if (dict.ContainsKey(year))
                    {
                        var yearData = dict[year];
                        yearData.overallGpa += gpa;
                        yearData.numberOfOccurrences += 1;
                        dict[year] = yearData;
                    }
                    else
                    {
                        dict.Add(year, (gpa, 1));
                    }
                }
            }

            return dict;
        }
    }
}
