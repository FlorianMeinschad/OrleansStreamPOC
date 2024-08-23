using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrleansPOC.Grains.Publisher;
using OrleansPOC.Grains.Subscriber;

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
}