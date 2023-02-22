# RPS Analysis

## No Tracing (OTEL and Custom Middleware disabled)

TelemetryOptions for this config
```json
"TelemetryOptions": {
    "EnableTelemetry": false,
    "UseOTelSDK": false, // Enables the path to use Otel SDK, when `false` the telemetry middleware is used
    "EnableTracing": false, // Enables tracing when UseOtelSDK flag is enabled
    "EnableHttpInstrumentation": false, // Enables Http auto instrumentation (For tracing/metering based on what's enabled)
    "EnableAspNetCoreInstrumentation": false, // Enable AspNetCore auto instrumentation (For tracing/metering based on what's enabled)
    "EnableMetering": false // Enables Metering
  }
```

```
C:\Users\mdeepak\Downloads>bombardier-windows-386.exe -c 1 -n 1000000 http://localhost:5000/weatherforecast/test
Bombarding http://localhost:5000/weatherforecast/test with 1000000 request(s) using 1 connection(s)
 1000000 / 1000000 [============================================================================] 100.00% 10060/s 1m39s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec     10077.26     544.65   10998.79
  Latency       98.12us    19.21us     7.00ms
  HTTP codes:
    1xx - 0, 2xx - 1000000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:     2.34MB/s
```

## AspNetCore tracing enabled and Telemetry captured via Custom Middleware (OTEL disabled)

TelemetryOptions for this config
```json
"TelemetryOptions": {
    "EnableTelemetry": true,
    "UseOTelSDK": false, // Enables the path to use Otel SDK, when `false` the telemetry middleware is used
    "EnableTracing": false, // Enables tracing when UseOtelSDK flag is enabled
    "EnableHttpInstrumentation": false, // Enables Http auto instrumentation (For tracing/metering based on what's enabled)
    "EnableAspNetCoreInstrumentation": false, // Enable AspNetCore auto instrumentation (For tracing/metering based on what's enabled)
    "EnableMetering": false // Enables Metering
  }
```

```
C:\Users\mdeepak\Downloads>bombardier-windows-386.exe -c 1 -n 1000000 http://localhost:5000/weatherforecast/test
Bombarding http://localhost:5000/weatherforecast/test with 1000000 request(s) using 1 connection(s)
 1000000 / 1000000 [=============================================================================================================] 100.00% 9663/s 1m43s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec      9680.67     807.55   10797.52
  Latency      102.15us    94.33us    92.10ms
  HTTP codes:
    1xx - 0, 2xx - 1000000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:     2.25MB/s
```

## OTEL Tracing enabled using OTEL AspNetCoreInstrumentation

TelemetryOptions for this config
```json
"TelemetryOptions": {
    "EnableTelemetry": true,
    "UseOTelSDK": true, // Enables the path to use Otel SDK, when `false` the telemetry middleware is used
    "EnableTracing": true, // Enables tracing when UseOtelSDK flag is enabled
    "EnableHttpInstrumentation": false, // Enables Http auto instrumentation (For tracing/metering based on what's enabled)
    "EnableAspNetCoreInstrumentation": true, // Enable AspNetCore auto instrumentation (For tracing/metering based on what's enabled)
    "EnableMetering": false // Enables Metering
  }
```

```
C:\Users\mdeepak\Downloads>bombardier-windows-386.exe -c 1 -n 1000000 http://localhost:5000/weatherforecast/test
Bombarding http://localhost:5000/weatherforecast/test with 1000000 request(s) using 1 connection(s)
 1000000 / 1000000 [=============================================================================] 100.00% 8595/s 1m56s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec      8610.81     934.58    9915.43
  Latency      114.95us    57.61us    15.50ms
  HTTP codes:
    1xx - 0, 2xx - 1000000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:     2.00MB/s
```

# Summary

* OTEL AspNetCore Tracing instrumentation is highly inefficient. It reduces the throughput by 10-15%.
