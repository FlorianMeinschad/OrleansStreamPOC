using Microsoft.Extensions.Logging;
using Orleans.Concurrency;

namespace OrleansPOC.Grains.HealthCheck;

public class HealthCheckGrain(ILogger<IHealthCheckGrain> logger, IGrainRuntime grainRuntime) : Grain, IHealthCheckGrain
{
    public Task CheckAsync()
    {
        return Task.CompletedTask;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HealthCheck Grain started successfully on silo {Silo}", grainRuntime.SiloAddress);
        this.RegisterGrainTimer<object>(_ => HealthLogAsync(), null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        logger.LogInformation("HealthCheck Grain stopped successfully on silo {Silo}, because of {@Reason}", grainRuntime.SiloAddress, reason);
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    private Task HealthLogAsync()
    {
        logger.LogInformation("HealthCheck Grain is still alive");
        return Task.CompletedTask;
    }
}