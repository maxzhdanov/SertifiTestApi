using System.Threading.Tasks;
using SertifiTestApi.Models;
using SertifiTestApi.Services;

namespace SertifiTestApi.Tests.Fakes
{
    public class TestAggregateService : IAggregateService
    {
        public async Task<StudentsAggregate> GetAggregate()
        {
            return new StudentsAggregate();
        }

        public async Task<bool> SubmitAggregate(StudentsAggregateSubmitRequest request)
        {
            return true;
        }
    }
}
