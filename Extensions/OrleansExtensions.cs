using System.Net;
using Microsoft.Extensions.Hosting;
using OrleansPOC.Config;
using OrleansPOC.Grains;
using Serilog;

namespace OrleansPOC.Extensions;

public static class OrleansConstants
{
    public const string InMemoryStorage = "InMemoryStore";
    public const string PubSubStore = "PubSubStore";
}

internal static class OrleansExtensions
{
    private const string ClusterId = "orleans-cluster";
    private const string ServiceId = "orleans-service";

    internal static void UseOrleansPoc(this IHostBuilder hostBuilder, SiloConfig config)
    {
        hostBuilder.UseOrleans(silo =>
        {
            silo.UseDashboard(options => {
                options.HostSelf = false;
            });

            Log.Information("Using Orleans Silo Port {Port}", config.SiloPort);
            Log.Information("Using Orleans Silo GW {GW}", config.SiloGateway);
            Log.Information("Use orleans localhost clustering");
            silo.UseLocalhostClustering(config.SiloPort, config.SiloGateway, new IPEndPoint(IPAddress.Loopback, config.PrimarySiloPort), ServiceId, ClusterId);

            silo.AddMemoryStreams(ArtisStreamProviderIds.STREAM);
            silo.AddMemoryGrainStorage(OrleansConstants.InMemoryStorage);
            silo.AddMemoryGrainStorage(OrleansConstants.PubSubStore);
        });
    }
}