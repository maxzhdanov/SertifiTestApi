using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SertifiTestApi.Filters;

namespace SertifiTestApi.Models
{
    public class StudentsAggregateSubmitRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailRegularExpression]
        public string Email { get; set; }
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
