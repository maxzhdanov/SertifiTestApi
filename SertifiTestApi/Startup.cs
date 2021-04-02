using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SertifiTestApi.Services;
using System.Net.Http;
using System.Reflection;
using SertifiTestApi.Middleware;
using Polly;
using SertifiTestApi.Clients;
using SertifiTestApi.Filters;

namespace SertifiTestApi
{
    public class Startup
    {
        private static readonly Assembly StartupAssembly = typeof(Startup).Assembly;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("*")
                        .AllowCredentials());
            });

            services.AddApplicationInsightsTelemetry();

            services.AddHttpClient("sertifi", c =>
            {
                c.BaseAddress = new Uri(Configuration["SertifiClient:BaseAddress"]);
                c.Timeout = TimeSpan.Parse(Configuration["SertifiClient:TimeOut"]);
            }).AddTransientHttpErrorPolicy(
                p => p.CircuitBreakerAsync(
                    Int32.Parse(Configuration["CircuitBreaker:RetriesCount"]),
                    TimeSpan.Parse(Configuration["CircuitBreaker:DurationOfBreak"])));
            
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            });

            services.AddScoped<IAggregateService, AggregateService>();
            services.AddScoped<ISertifiHttpClient, SertifiHttpClient>(s => new SertifiHttpClient(
                      s.GetService<IHttpClientFactory>()
                ));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "SertifiTest API";
                    document.Info.Description = $"Build version={StartupAssembly.GetName().Version}";
                };
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");

            app.UseMiddleware<ErrorLoggingMiddleware>();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SertifiTest API V1");
            });

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
