namespace TelemetryPerfImpactAnalysis;

public class TelemetryOptions
{
    public bool EnableTelemetry { get; set; }
    
    public bool UseOTelSDK { get; set; }

    public bool EnableTracing { get; set; }

    public bool EnableMetering { get; set; }

    public bool EnableAspNetCoreInstrumentation { get; set; }

    public bool EnableHttpInstrumentation { get; set; }
}
