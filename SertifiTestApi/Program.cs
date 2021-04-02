using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SertifiTestApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            // This will be picked up by AI
            logger.LogInformation("From Program. Running the host now..");
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureLogging(
                                                builder =>
                                                {
                                                    // Providing an instrumentation key here is required if you're using
                                                    // standalone package Microsoft.Extensions.Logging.ApplicationInsights
                                                    // or if you want to capture logs from early in the application startup 
                                                    // pipeline from Startup.cs or Program.cs itself.
                                                    builder.AddApplicationInsights("");

                                                    // Adding the filter below to ensure logs of all severity from Program.cs
                                                    // is sent to ApplicationInsights.
                                                    builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                                                     (typeof(Program).FullName, LogLevel.Trace);

                                                    // Adding the filter below to ensure logs of all severity from Startup.cs
                                                    // is sent to ApplicationInsights.
                                                    builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                                                     (typeof(Startup).FullName, LogLevel.Trace);
                                                }
                                               );
                });
    }
}
