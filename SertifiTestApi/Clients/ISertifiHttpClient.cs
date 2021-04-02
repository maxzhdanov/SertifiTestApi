using System.Collections.Generic;
using System.Threading.Tasks;
using SertifiTestApi.Models;

namespace SertifiTestApi.Clients
{
    public interface ISertifiHttpClient
    {
        Task<bool> SubmitAggregate(StudentsAggregateSubmitRequest request);

        Task<IReadOnlyList<StudentRecordResponse>> GetStudentRecords();
    }
}
