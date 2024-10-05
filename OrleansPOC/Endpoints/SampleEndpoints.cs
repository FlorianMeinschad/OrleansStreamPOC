using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrleansPOC.Grains;
using OrleansPOC.Grains.HealthCheck;
using OrleansPOC.Grains.Publisher;
using OrleansPOC.Grains.Subscriber;
using Streams.Extensions;

namespace OrleansPOC.Endpoints;

public static class SampleEndpoints
{
    public static async Task<Ok<string>> StartPublisherAsync([FromRoute] int intervalInSeconds, IGrainFactory grainFactory, ILogger<IEndpointLogger> logger)
    {
        logger.LogInformation("Starting publisher grain");

        var publisherGrain = grainFactory.GetGrain<IPublisherGrain>(Guid.Empty);
        await publisherGrain.StartAsync(TimeSpan.FromSeconds(intervalInSeconds));

        logger.LogInformation("Publisher grain started with Id {GrainId}", publisherGrain.GetPrimaryKey());
        return TypedResults.Ok($"Publishing grain started with Id {publisherGrain.GetPrimaryKey()}");
    }

    public static async Task<Ok<string>> StartSubscribersAsync([FromRoute] int numOfSubs, IGrainFactory grainFactory, ILogger<IEndpointLogger> logger)
    {
        logger.LogInformation("Starting subscriber grains");

        for (var i = 0; i < numOfSubs; i++)
        {
            var subscriberGrain = grainFactory.GetGrain<ISubscriberGrain>(Guid.NewGuid());
            await subscriberGrain.StartAsync();
        }

        logger.LogInformation("{NumOfSubscribers} subscriber grains started", numOfSubs);
        return TypedResults.Ok($"{numOfSubs} subscriber grains started");
    }

    public static async Task<Ok<string>> StopSubscriberByGrainId([FromRoute] Guid grainId, IGrainFactory grainFactory, ILogger<IEndpointLogger> logger)
    {
        logger.LogInformation("Stopping subscriber grain {GrainId}", grainId);
        var grain = grainFactory.GetGrain<ISubscriberGrain>(grainId);
        await grain.StopAsync();
        return TypedResults.Ok($"Subscriber grain {grainId} stopped");
    }

    public static async Task<Ok<string>> StopSubscriberByGrainButKeepSubscriptionId([FromRoute] Guid grainId, IGrainFactory grainFactory, ILogger<IEndpointLogger> logger)
    {
        logger.LogInformation("Stopping subscriber grain {GrainId}", grainId);
        var grain = grainFactory.GetGrain<ISubscriberGrain>(grainId);
        await grain.StopButKeepSubscriptionAsync();
        return TypedResults.Ok($"Subscriber grain {grainId} stopped");
    }

    public static async Task<Ok<string>> PublishSingleMessageAsync([FromRoute] string message, ILocalSiloDetails localSiloDetails, IClusterClient clusterClient, ILogger<IEndpointLogger> logger)
    {
        logger.LogInformation("Publishing single message");
        var streamProvider = clusterClient.GetArtisStreamProvider(StreamProviderIds.STREAM);
        var stream = streamProvider.GetStream<string>(StreamChannelIds.TEST_STREAM_ID);
        await stream.OnNextAsync("Message from endpoint: " + message);
        logger.LogInformation("Send single message from endpoint on silo {Silo}: {Message}", localSiloDetails.SiloAddress, message);
        return TypedResults.Ok("Message published successfully");
    }

    public static async Task<ContentHttpResult> GetAllSubscriptionsAsync(ILocalSiloDetails localSiloDetails, IClusterClient clusterClient, ILogger<IEndpointLogger> logger)
    {
        logger.LogInformation("Retrieving all subscribers from stream {StreamId} single message", StreamChannelIds.TEST_STREAM_ID);
        var streamProvider = clusterClient.GetArtisStreamProvider(StreamProviderIds.STREAM);
        var stream = streamProvider.GetStream<string>(StreamChannelIds.TEST_STREAM_ID);
        var subs = string.Join(Environment.NewLine, (await stream.GetAllSubscriptionsAsync()).Select(x => $"{x.SiloAddress.ToString()}: {x.StreamId} - {x.SubscriptionId}").ToArray());
        return TypedResults.Content(subs, "text/plain");
    }

    public static async Task<Ok<string>> StartHealthChecksAsync(IGrainFactory grainFactory)
    {
        var grain = grainFactory.GetGrain<IHealthCheckGrain>(Guid.Empty);
        await grain.CheckAsync();
        return TypedResults.Ok("Health check grain started successfully");
    }
}