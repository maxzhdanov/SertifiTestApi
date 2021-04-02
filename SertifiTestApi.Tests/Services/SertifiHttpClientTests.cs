using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Contrib.HttpClient;
using SertifiTestApi.Clients;
using SertifiTestApi.Models;
using Xunit;

namespace SertifiTestApi.Tests.Services
{
    public class SertifiHttpClientTests
    {
        private readonly Mock<HttpMessageHandler> _handler;
        private readonly IHttpClientFactory _factory;
        private readonly string _url = "http://sertifi";
        private readonly string _studentsDataJSon = @"[
            {
              ""Id"": 1,
              ""Name"": ""Jack"",
              ""StartYear"": 2013,
              ""EndYear"": 2016,
              ""GPARecord"": [3.4,2.1,1.2,3.6]
            },
            {
              ""Id"": 2,
              ""Name"": ""Jill"",
              ""StartYear"": 2010,
              ""EndYear"": 2013,
              ""GPARecord"": [3.3,2.3,1.1,3.7]
            }
        ]";

        public SertifiHttpClientTests()
        {
            _handler = new Mock<HttpMessageHandler>();
            _factory = _handler.CreateClientFactory();

            Mock.Get(_factory).Setup(x => x.CreateClient("sertifi"))
                .Returns(() =>
                {
                    var client = _handler.CreateClient();
                    client.BaseAddress = new Uri($"{_url}");
                    return client;
                });
        }

        [Fact]
        public async Task GetStudentRecords_Success()
        {
            var expectedResult = new List<StudentRecordResponse>
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

            _handler.SetupRequest($"{_url}/students")
                .Returns(async (HttpRequestMessage request, CancellationToken _) => new HttpResponseMessage()
                {
                    Content = new StringContent(_studentsDataJSon, Encoding.UTF8, "application/json")
                });

            var service = new SertifiHttpClient(_factory);

            var result = await service.GetStudentRecords();

            Assert.NotNull(result);
            Assert.Equal(expectedResult.Count, result.Count);
            Assert.Equal(expectedResult[0].Name, result[0].Name);
        }

        [Fact]
        public async Task UpdateUserEmail_Success()
        {
            _handler.SetupRequest($"{_url}/studentaggregate")
                .Returns(async (HttpRequestMessage request, CancellationToken _) => new HttpResponseMessage()
                {
                    Content = new StringContent("true", Encoding.UTF8, "text/plain")
                });

            var service = new SertifiHttpClient(_factory);

            var result = await service.SubmitAggregate(new StudentsAggregateSubmitRequest
            {
                Name = "Subm Itter",
                Email = "submitter@test.com",
                YearWithHighestAttendance = 2999,
                YearWithHighestAverageGpa = 2999,
                Top10StudentIdsWithHighestGpa = new List<int> { 3, 2, 3 },
                StudentIdMostInconsistent = 1
            });

            Assert.True(result);
        }
    }
}
