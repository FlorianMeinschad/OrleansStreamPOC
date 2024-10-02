using System.Net;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using OrleansPOC.Config;
using OrleansPOC.Grains;
using Serilog;

namespace OrleansPOC.Extensions;

public static class OrleansConstants
{
    public const string InMemoryStorage = "InMemoryStore";
}

internal static class OrleansExtensions
{
    private const string ClusterId = "orleans-cluster";
    private const string ServiceId = "orleans-service";

    private const string DefaultStoreName = "DefaultStore";
    private const string ProviderName = "System.Data.SqlClient";

    private const string SqlConnectionString =
        "Server=127.0.0.1,10433;Database=Orleans_DEBUG;User Id=sa;Password=Password123!;TrustServerCertificate=true";

    internal static void UseOrleansPoc(this IHostBuilder hostBuilder, SiloConfig config)
    {
        bool useInMemory = false;

        hostBuilder.UseOrleans(silo =>
        {
            silo.UseDashboard(options => {
                options.HostSelf = false;
            });

            Log.Information("Using Orleans Silo Port {Port}", config.SiloPort);
            Log.Information("Using Orleans Silo GW {GW}", config.SiloGateway);

            if (useInMemory)
            {
                Log.Information("Use orleans in memory clustering");
                silo.UseLocalhostClustering(config.SiloPort, config.SiloGateway, new IPEndPoint(IPAddress.Loopback, config.PrimarySiloPort), ServiceId, ClusterId);
            }
            else
            {
                Log.Error("Using ADO.NET clustering");

                silo.ConfigureEndpoints(config.SiloPort, config.SiloGateway);

                silo.Configure<ClusterOptions>(options => {
                    options.ClusterId = ClusterId;
                    options.ServiceId = ServiceId;
                });

                silo.UseAdoNetClustering(options => {
                    options.Invariant = ProviderName;
                    options.ConnectionString = SqlConnectionString;
                });

                silo.AddAdoNetGrainStorage(DefaultStoreName, options => {
                    options.Invariant = ProviderName;
                    options.ConnectionString = SqlConnectionString;
                });
            }

            silo.AddMemoryStreams(StreamProviderIds.STREAM);
            silo.AddMemoryGrainStorage("PubSubStore");
            silo.AddMemoryGrainStorage(OrleansConstants.InMemoryStorage);
        });
    }
}