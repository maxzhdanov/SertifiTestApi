using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SertifiTestApi.Models;
using SertifiTestApi.Services.Extensions;

namespace SertifiTestApi.Clients
{
    public class SertifiHttpClient : ISertifiHttpClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private const string SertifiClient = "sertifi";
        private const string Students = "students";
        private const string StudentAggregate = "studentaggregate";

        public SertifiHttpClient(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> SubmitAggregate(StudentsAggregateSubmitRequest record)
        {
            var client = _clientFactory.CreateClient(SertifiClient);

            var uri = $"{client.BaseAddress}{StudentAggregate}";

            using var response = await client.PutAsJsonAsync(uri, record);

            response.ValidateStatusCode();

            return true;
        }


        public async Task<IReadOnlyList<StudentRecordResponse>> GetStudentRecords()
        {
            var client = _clientFactory.CreateClient(SertifiClient);

            var uri = $"{client.BaseAddress}{Students}";

            using var response = await client.GetAsync(uri);

            response.ValidateStatusCode();

            return await response.Content.ReadAsAsync<List<StudentRecordResponse>>();
        }
    }
}
