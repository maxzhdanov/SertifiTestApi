using System.Collections.Generic;
using FluentAssertions;
using System.Net.Http;
using System.Threading.Tasks;
using SertifiTestApi.Internal.Tests;
using SertifiTestApi.Models;
using SertifiTestApi.Tests.Internal;
using Xunit;

namespace SertifiTestApi.Tests.Controllers
{
    [Collection("Api")]
    public class StudentsControllerTests : IClassFixture<WebApplicationFixture>
    {
        private const string ControllerUrl = "api/students";
        private readonly HttpClient _client;

        public StudentsControllerTests(WebApplicationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GetAggregate_ShouldSucceed()
        {
            // act
            var response = await _client.GetAsync($"{ControllerUrl}/GetAggregate");

            // assert
            response.StatusCode.Should().Be(200);
            await response.Content.ReadAsAsync<StudentsAggregate>();
        }

        [Fact]
        public async Task SubmitAggregate_ShouldSucceed()
        {
            // arrange
            var record = new StudentsAggregateSubmitRequest
            {
                Name = "Subm Itter",
                Email = "submitter@test.com",
                YearWithHighestAttendance = 2999,
                YearWithHighestAverageGpa = 2999,
                Top10StudentIdsWithHighestGpa = new List<int> { 3, 2, 3 },
                StudentIdMostInconsistent = 1
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"{ControllerUrl}/SubmitAggregate")
            {
               Content = new JsonContent(record)
            };

            // act
            var response = await _client.SendAsync(request);

            // assert
            response.StatusCode.Should().Be(200);
            var result = await response.Content.ReadAsAsync<bool>();
            result.Should().BeTrue();
        }

        [Fact]
        public async Task SubmitAggregate_ValidationMessages_ShouldBeReturned()
        {
            // arrange
            var record = new StudentsAggregateSubmitRequest
            {
                Name = "",
                Email = "submitter@test",
                YearWithHighestAttendance = 2999,
                YearWithHighestAverageGpa = 2999,
                StudentIdMostInconsistent = 1
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"{ControllerUrl}/SubmitAggregate")
            {
                Content = new JsonContent(record)
            };

            // act
            var response = await _client.SendAsync(request);

            // assert
            response.StatusCode.Should().Be(400);
            var raw = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("The Name field is required.", raw);
            Assert.Contains("The Top10StudentIdsWithHighestGpa field is required.", raw);
            Assert.Contains("[submitter@test] is not a valid email.", raw);
        }
    }
}
