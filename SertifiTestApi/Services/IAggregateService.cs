using System.Threading.Tasks;
using SertifiTestApi.Models;

namespace SertifiTestApi.Services
{
    public interface IAggregateService
    {
        Task<StudentsAggregate> GetAggregate();

        Task<bool> SubmitAggregate(StudentsAggregateSubmitRequest request);
    }
}
