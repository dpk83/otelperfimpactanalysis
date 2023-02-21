using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.Metrics;

namespace TelemetryPerfImpactAnalysis;

public class TelemetryMiddleware : IMiddleware
{
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;

    public TelemetryMiddleware()
    {
        _meter = new Meter("TelemetryMiddleware");
        _requestCounter = _meter.CreateCounter<long>("requestCounter");
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var activity = Activity.Current;
            //var activityFeature = context.Features.Get<IHttpActivityFeature>();
            //var act = activityFeature.Activity;
            var baggage = context.Request.Headers["baggage"];

            await next(context).ConfigureAwait(false);
            OnRequestEnd(context, context.Response.StatusCode, null);
        }
        catch (Exception ex)
        {
            int resultCode = context.Response.StatusCode < StatusCodes.Status400BadRequest ? StatusCodes.Status500InternalServerError : context.Response.StatusCode;
            OnRequestEnd(context, resultCode, ex.GetType());

            throw;
        }
    }

    private void OnRequestEnd(HttpContext httpContext, int resultCode, Type? exceptionType)
    {
        var activity = Activity.Current;
        var endpoint = httpContext.GetEndpoint() as RouteEndpoint;

        var request = httpContext.Request;
        string host = request.Host.Value;
        string method = request.Method;
        string route = endpoint?.RoutePattern.RawText ?? "unsupported_route";
        string responseResultCode = resultCode.ToString();

        activity.SetTag("http.host", host);
        activity.SetTag("http.method", method);
        activity.SetTag("http.route", route);
        activity.SetTag("http.status_code", responseResultCode);

        _requestCounter.Add(1, new TagList
        {
            new ("http.host", host),
            new ("http.method", method),
            new ("http.route", route),
            new ("http.status_code", responseResultCode)
        });
    }
}

