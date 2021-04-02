using System.Collections.Generic;

namespace SertifiTestApi.Models
{
    public class StudentsAggregate
    {
        public int YearWithHighestAttendance { get; set; }
        public int YearWithHighestAverageGpa { get; set; }
        public List<int> Top10StudentIdsWithHighestGpa { get; set; }
        public int StudentIdMostInconsistent { get; set; }
    }
}
