## POC: Orleans Streams Failing After Silo Crash

This proof of concept (POC) demonstrates an issue where Orleans streams stop functioning after one of the silos crashes or shuts down. 
Although the other silo remains running, it no longer receives messages from the publisher.

Urls:
 - Dashboard: http://localhost:5000/dashboard
 - Start Publisher: http://localhost:5000/pub/{intervalInSeconds}
 - Start Subscriber: http://localhost:5000/sub/{numOfSubs}

### POC steps (try multiple times, unfortunately not working always):

1. Start Silo1 and call the following URLs: <br> 
   http://localhost:5000/pub/5 <br> 
   http://localhost:5000/sub/3
2. Start Silo2 and call the following URL: <br>
   http://localhost:5000/sub/3
3. You can also check the dashboard to confirm that grains are hosted on both silos <br>
   http://localhost:5000/dashboard 
4. Review the logs for both Silo1 and Silo2. Each silo should receive messages from the publisher every 5 seconds
5. Terminate Silo2 (press Ctrl+C in the console multiple times)
6. Check the log for Silo1. All subscribers will have stopped, and no further messages will be received from the publisher

-----------------------------------------------------------------------------------------------------------------
Sometimes the following exceptions are thrown:

Logs of Silo2
```
[13:16:22 INF] Subscriber Grain 791e0ed160214841ad5d8792d290bc19 on Silo S127.0.0.1:20000:83416570 received publication Message sent from publisher
[13:16:28 INF] Stopping OrleansDashboard.SiloGrainService grain service
[13:16:28 INF] Subscriber stopped successfully
[13:16:28 ERR] Error calling grain's OnDeactivateAsync(...) method - Grain type = OrleansPOC.Grains.Subscriber.SubscriberGrain Activation = [Activation: S127.0.0.1:20000:83416570/subscriber/d75b1277d7c7457cab20e5475a994a87@988280c73309451588149aa4cf1c87f0#GrainType=OrleansPOC.Grains.Subscriber.SubscriberGrain,OrleansPOC Placement=RandomPlacement State=Deactivating]
Orleans.Runtime.OrleansMessageRejectionException: Forwarding failed: tried to forward message Request [S127.0.0.1:20000:83416570 subscriber/d75b1277d7c7457cab20e5475a994a87]->[S127.0.0.1:11111:83416554 pubsubrendezvous/STREAM_EXAMPLE/null/STREAM] Orleans.Streams.IPubSubRendezvousGrain.UnregisterConsumer(2ddc34f6-3cea-4bf9-98d0-9049bb42b957, STREAM_EXAMPLE/null/STREAM) #1036[ForwardCount=2] for 2 times after "DeactivateOnIdle was called." to invalid activation. Rejecting now.
   at Orleans.Serialization.Invocation.ResponseCompletionSource.System.Threading.Tasks.Sources.IValueTaskSource.GetResult(Int16 token) in /_/src/Orleans.Serialization/Invocation/ResponseCompletionSource.cs:line 98
   at System.Threading.Tasks.ValueTask.ValueTaskSourceAsTask.<>c.<.cctor>b__4_0(Object state)
--- End of stack trace from previous location ---
   at Orleans.Streams.StreamConsumer`1.UnsubscribeAsync(StreamSubscriptionHandle`1 handle) in /_/src/Orleans.Streaming/Internal/StreamConsumer.cs:line 183
   at OrleansPOC.Grains.Subscriber.SubscriberGrain.OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken) in D:\OrleansPOC\OrleansPOC\Grains\Subscriber\SubscriberGrain.cs:line 44
   at Orleans.Internal.OrleansTaskExtentions.MakeCancellable(Task task, CancellationToken cancellationToken) in /_/src/Orleans.Core/Async/TaskExtensions.cs:line 187
   at Orleans.Internal.OrleansTaskExtentions.WithCancellation(Task taskToComplete, String message, CancellationToken cancellationToken) in /_/src/Orleans.Core/Async/TaskExtensions.cs:line 144
   at Orleans.Runtime.ActivationData.<FinishDeactivating>g__CallGrainDeactivate|145_0(CancellationToken ct) in /_/src/Orleans.Runtime/Catalog/ActivationData.cs:line 1799
[13:16:30 INF] Host terminated successfully
```

Logs of Silo1
```
[13:16:17 INF] Subscriber Grain 4a1dafd47e4940d28b1b695d3f7a4e8c on Silo S127.0.0.1:11111:83416554 received publication Message sent from publisher
[13:16:22 INF] Subscriber Grain 4a1dafd47e4940d28b1b695d3f7a4e8c on Silo S127.0.0.1:11111:83416554 received publication Message sent from publisher
[13:16:22 INF] Subscriber Grain 3e23f90d920d47a9b665edb9c44bbbd7 on Silo S127.0.0.1:11111:83416554 received publication Message sent from publisher
[13:16:28 INF] My range changed from <MultiRange: Size=x8E8388F1, %Ring=55,669%> to <(0 0], Size=x100000000, %Ring=100%> increased = True
[13:16:28 INF] Subscriber stopped successfully
[13:16:28 INF] Subscriber stopped successfully
[13:16:28 WRN] Failed to forward message Request [S127.0.0.1:20000:83416570 subscriber/d75b1277d7c7457cab20e5475a994a87]->[S127.0.0.1:11111:83416554 pubsubrendezvous/STREAM_EXAMPLE/null/STREAM] Orleans.Streams.IPubSubRendezvousGrain.UnregisterConsumer(2ddc34f6-3cea-4bf9-98d0
-9049bb42b957, STREAM_EXAMPLE/null/STREAM) #1036[ForwardCount=2] from [GrainAddress GrainId pubsubrendezvous/STREAM_EXAMPLE/null/STREAM, ActivationId: @436c6408319b4230b861f9e3fcb0b55a, SiloAddress: S127.0.0.1:11111:83416554] to null after DeactivateOnIdle was called.. Attempt 2
```