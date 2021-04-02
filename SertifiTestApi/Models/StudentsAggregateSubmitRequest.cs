using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SertifiTestApi.Filters;

namespace SertifiTestApi.Models
{
    public class StudentsAggregateSubmitRequest
    {
        [Required]
        public string YourName { get; set; }
        [Required]
        [EmailRegularExpression]
        public string YourEmail { get; set; }
        [Required]
        public int YearWithHighestAttendance { get; set; }
        [Required]
        public int YearWithHighestAverageGpa { get; set; }
        [Required]
        public List<int> Top10StudentIdsWithHighestGpa { get; set; }
        [Required]
        public int StudentIdMostInconsistent { get; set; }
    }
}
