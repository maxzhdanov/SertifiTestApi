using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SertifiTestApi.Clients;
using SertifiTestApi.Models;

namespace SertifiTestApi.Tests.Fakes
{
    public class TestSertifiHttpClient : ISertifiHttpClient
    {
        public Task<bool> SubmitAggregate(StudentsAggregateSubmitRequest record)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<StudentRecordResponse>> GetStudentRecords()
        {
            return new List<StudentRecordResponse>
            {
                new StudentRecordResponse
                {
                    Id = 1, Name = "Jack", StartYear = 2013, EndYear = 2016, GPARecord = new List<double> {3.4,2.1,1.2,3.6}
                },
                new StudentRecordResponse
                {
                    Id = 2, Name = "Jill", StartYear = 2010, EndYear = 2013, GPARecord = new List<double> {3.3,2.3,1.1,3.7}
                }
            };
        }
    }
}
