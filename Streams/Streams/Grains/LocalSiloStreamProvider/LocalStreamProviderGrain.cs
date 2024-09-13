using Orleans.Runtime;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace Streams.Grains.LocalSiloStreamProvider;

public class LocalStreamProviderGrain : ILocalStreamProviderGrain, IStreamPubSub
{
    public Task<ISet<PubSubSubscriptionState>> RegisterProducer(QualifiedStreamId streamId, GrainId streamProducer)
    {
        throw new NotImplementedException();
    }

    public Task UnregisterProducer(QualifiedStreamId streamId, GrainId streamProducer)
    {
        throw new NotImplementedException();
    }

    public Task RegisterConsumer(GuidId subscriptionId, QualifiedStreamId streamId, GrainId streamConsumer, string filterData)
    {
        throw new NotImplementedException();
    }

    public Task UnregisterConsumer(GuidId subscriptionId, QualifiedStreamId streamId)
    {
        throw new NotImplementedException();
    }

    public Task<int> ProducerCount(QualifiedStreamId streamId)
    {
        throw new NotImplementedException();
    }

    public Task<int> ConsumerCount(QualifiedStreamId streamId)
    {
        throw new NotImplementedException();
    }

    public Task<List<StreamSubscription>> GetAllSubscriptions(QualifiedStreamId streamId, GrainId streamConsumer = new GrainId())
    {
        throw new NotImplementedException();
    }

    public GuidId CreateSubscriptionId(QualifiedStreamId streamId, GrainId streamConsumer)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FaultSubscription(QualifiedStreamId streamId, GuidId subscriptionId)
    {
        throw new NotImplementedException();
    }
}