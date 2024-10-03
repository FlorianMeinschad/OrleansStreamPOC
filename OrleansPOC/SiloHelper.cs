﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OrleansPOC.Config;
using OrleansPOC.Endpoints;
using OrleansPOC.Extensions;
using OrleansPOC.Grains.HealthCheck;
using Serilog;
using Serilog.Events;
using Streams.Extensions;

namespace OrleansPOC;

public static class SiloHelper
{
    public static void Startup(SiloConfig config)
    {
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder([]);
            builder.WebHost.UseUrls(config.Urls);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console().MinimumLevel.Debug()
                .MinimumLevel.Override("Orleans", LogEventLevel.Warning)
                .MinimumLevel.Override("System.Net", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .CreateLogger();

            Log.Information("Running in environment {Environment}", builder.Environment.EnvironmentName);
            builder.Host.UseOrleansPoc(config);
            builder.Host.UseSerilog();
            builder.Services.AddArtisStreaming();

            WebApplication app = builder.Build();
            app.Map("/dashboard", x => x.UseOrleansDashboard());

            app.Map("/pub/{intervalInSeconds:int}", SampleEndpoints.StartPublisherAsync);
            app.Map("/sub/{numOfSubs:int}", SampleEndpoints.StartSubscribersAsync);
            app.Map("/pub/message/{message}", SampleEndpoints.PublishSingleMessageAsync);
            app.Map("/pub/stop/{grainId:guid}", SampleEndpoints.StopSubscriberByGrainId);
            app.Map("/pub/stopGrainOnly/{grainId:guid}", SampleEndpoints.StopSubscriberByGrainButKeepSubscriptionId);
            app.Map("/sub/list", SampleEndpoints.GetAllSubscriptionsAsync);
            app.Map("/health", SampleEndpoints.StartHealthChecksAsync);

            Log.Information("Starting Host");
            app.Run();
            Log.Information("Host terminated successfully");
        } catch (Exception e) {
            Console.Error.WriteLine(e);
            Log.Fatal(e, "Host terminated unexpectedly");
        } finally {
            Log.CloseAndFlush();
        }
    }
}