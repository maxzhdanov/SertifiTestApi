using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using AutoFixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SertifiTestApi.Clients;
using SertifiTestApi.Services;
using SertifiTestApi.Tests.Fakes;

namespace SertifiTestApi.Tests.Internal
{
    public class WebApplicationFixture : IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IConfigurationRoot Configuration { get; set; }
        public HttpClient Client { get; }
        public Fixture Fixture { get; private set; }

        public WebApplicationFixture()
        {
            _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(BuildHost);
            Client = _factory.CreateClient();

            Fixture = new Fixture();
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        private void BuildHost(IWebHostBuilder host)
        {
            host.ConfigureAppConfiguration(ConfigureAppConfiguration);
            host.ConfigureTestServices(ConfigureTestServices);
        }

        private void ConfigureAppConfiguration(IConfigurationBuilder builder)
        {
            Configuration = builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();
        }

        private void ConfigureTestServices(IServiceCollection services)
        {
            services.AddScoped<IAggregateService, TestAggregateService>();
            services.AddScoped<ISertifiHttpClient, TestSertifiHttpClient>();
        }

        public void Dispose()
        {
            _factory?.Dispose();
            Client?.Dispose();
        }
    }
}
