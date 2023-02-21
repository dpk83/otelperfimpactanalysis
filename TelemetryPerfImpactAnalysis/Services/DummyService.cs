using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace TelemetryPerfImpactAnalysis.Services;

public class DummyService
{
    private readonly HttpClient _httpClient;
    private readonly TelemetryOptions _telemetryOptions;
    public DummyService(IOptions<TelemetryOptions> telemetryOptions)
    {
        _httpClient = new HttpClient
        {
        };
        _telemetryOptions = telemetryOptions.Value;
    }

    public async Task<HttpResponseMessage> Get()
    {
        if (_telemetryOptions.EnableHttpInstrumentation)
        {
            return await _httpClient.GetAsync(new Uri("http://localhost:5000/weatherforecast/dummy", UriKind.Absolute));
        }

        return null!;
    }
}
