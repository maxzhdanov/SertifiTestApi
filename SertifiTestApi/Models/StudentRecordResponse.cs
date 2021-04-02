using System.Collections.Generic;

namespace SertifiTestApi.Models
{
    public class StudentRecordResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<double> GPARecord { get; set; }
    }
}
