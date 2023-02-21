using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using TelemetryPerfImpactAnalysis.Services;

namespace TelemetryPerfImpactAnalysis
{
    public class Startup
    {
        private readonly TelemetryOptions _telemetryOptions;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var section = Configuration.GetSection("TelemetryOptions");
            _telemetryOptions = new TelemetryOptions();
            section.Bind(_telemetryOptions);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<TelemetryOptions>(Configuration.GetSection("TelemetryOptions"));
            services.AddSingleton<DummyService>();

            if (_telemetryOptions.EnableTelemetry)
            {
                if (_telemetryOptions.UseOTelSDK)
                {
                    if (_telemetryOptions.EnableTracing)
                    {
                        _ = services.AddOpenTelemetry().WithTracing(builder =>
                        {
                            if (_telemetryOptions.EnableAspNetCoreInstrumentation)
                            {
                                _ = builder.AddAspNetCoreInstrumentation();
                            }

                            if (_telemetryOptions.EnableHttpInstrumentation)
                            {
                                _ = builder.AddHttpClientInstrumentation();
                            }
                        });
                    }

                    if (_telemetryOptions.EnableMetering)
                    {
                        _ = services.AddOpenTelemetry().WithMetrics(builder =>
                        {
                            if (_telemetryOptions.EnableAspNetCoreInstrumentation)
                            {
                                _ = builder.AddAspNetCoreInstrumentation();
                            }

                            if (_telemetryOptions.EnableHttpInstrumentation)
                            {
                                _ = builder.AddHttpClientInstrumentation();
                            }
                        });
                    }
                }
                else
                {
                    services.AddSingleton<TelemetryMiddleware>();

                    ActivitySource.AddActivityListener(new ActivityListener
                    {
                        ShouldListenTo = source => source.Name == "Microsoft.AspNetCore",
                        Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData,
                        ActivityStarted = activity => { },
                        ActivityStopped = activity => { },
                    });
                }
            }

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            if (_telemetryOptions.EnableTelemetry && !_telemetryOptions.UseOTelSDK)
            {
                app.UseMiddleware<TelemetryMiddleware>();
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
