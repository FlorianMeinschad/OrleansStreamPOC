## POC: Orleans Streams Failing After Silo Crash

This proof of concept (POC) demonstrates an issue where Orleans streams cease functioning after one of the silos crashes or shuts down. 
Although the remaining silo continues to run, it no longer receives messages from the publisher.

Urls:
 - Dashboard: http://localhost:5000/dashboard
 - Start Publisher: http://localhost:5000/pub/{intervalInSeconds}
 - Start Subscriber: http://localhost:5000/sub/{numOfSubs}

### POC steps (The issue may not occur every time; multiple attempts might be required):

1. **Start Silo1** and execute the following URLs: <br> 
   http://localhost:5000/pub/5 <br> 
   http://localhost:5000/sub/3
2. **Start Silo2** and execute the following URL: <br>
   http://localhost:5000/sub/3
3. **Optional** Use the dashboard to verify that grains are hosted on both silos <br>
   http://localhost:5000/dashboard 
4. **Monitor the logs** for both Silo1 and Silo2. You should see that both silos are receiving messages from the publisher every 5 seconds.
5. **Stop Silo2** by pressing CTRL+C in its console
6. **Check the log for Silo1**. You will observe that all subscribers have stopped, and Silo1 no longer receives messages from the publisher.

-----------------------------------------------------------------------------------------------------------------
Log of Silo2:
```
[13:36:18 INF] Running in environment Development
[13:36:18 INF] Using Orleans Silo Port 20000
[13:36:18 INF] Using Orleans Silo GW 40000
[13:36:18 INF] Use orleans localhost clustering
[13:36:19 INF] Starting Host
[13:36:19 WRN] Note: Silo not running with ServerGC turned on - recommend checking app config : <configuration>-<runtime>-<gcServer enabled="true">
[13:36:19 WRN] Note: ServerGC only kicks in on multi-core systems (settings enabling ServerGC have no effect on single-core machines).
[13:36:20 INF] Starting OrleansDashboard.SiloGrainService grain service on: S127.0.0.1:20000:83417778 x4F1BA0D3, with range <MultiRange: Size=x800B15E6, %Ring=50,017%>
[13:36:20 INF] My range changed from <(0 0], Size=x100000000, %Ring=100%> to <MultiRange: Size=x800B15E6, %Ring=50,017%> increased = True
[13:36:21 INF] Subscriber started successfully on Sile S127.0.0.1:20000:83417778
[13:36:21 INF] Subscriber started successfully on Sile S127.0.0.1:20000:83417778
[13:36:21 INF] Subscriber started successfully on Sile S127.0.0.1:20000:83417778
[13:36:24 INF] Subscriber Grain f7dbf895ee1345f8b3893e77c74375ae on Silo S127.0.0.1:20000:83417778 received publication Message sent from publisher
[13:36:24 INF] Subscriber Grain 8d8879bdae8d450a9ff87f26e2ad69b0 on Silo S127.0.0.1:20000:83417778 received publication Message sent from publisher
[13:36:24 INF] Subscriber Grain 05d60f264237439fb321ea7f2139743c on Silo S127.0.0.1:20000:83417778 received publication Message sent from publisher
[13:36:29 INF] Subscriber Grain 8d8879bdae8d450a9ff87f26e2ad69b0 on Silo S127.0.0.1:20000:83417778 received publication Message sent from publisher
[13:36:29 INF] Subscriber Grain 05d60f264237439fb321ea7f2139743c on Silo S127.0.0.1:20000:83417778 received publication Message sent from publisher
[13:36:29 INF] Subscriber Grain f7dbf895ee1345f8b3893e77c74375ae on Silo S127.0.0.1:20000:83417778 received publication Message sent from publisher
[13:36:38 INF] Stopping OrleansDashboard.SiloGrainService grain service
[13:36:38 INF] Subscriber stopped successfully
[13:36:38 INF] Subscriber stopped successfully
[13:36:38 INF] Subscriber stopped successfully
[13:36:41 INF] Host terminated successfully
```

Log of Silo1 (no more publisher messages are received):
```
Sometimes there are also following exceptions thrown:
[13:36:02 INF] Using Orleans Silo Port 11111
[13:36:02 INF] Using Orleans Silo GW 30000
[13:36:02 INF] Use orleans localhost clustering
[13:36:03 INF] Starting Host
[13:36:03 WRN] Note: Silo not running with ServerGC turned on - recommend checking app config : <configuration>-<runtime>-<gcServer enabled="true">
[13:36:03 WRN] Note: ServerGC only kicks in on multi-core systems (settings enabling ServerGC have no effect on single-core machines).
[13:36:04 INF] Starting OrleansDashboard.SiloGrainService grain service on: S127.0.0.1:11111:83417762 x9341EC4D, with range <(0 0], Size=x100000000, %Ring=100%>
[13:36:04 INF] My range changed from <(0 0], Size=x100000000, %Ring=100%> to <(0 0], Size=x100000000, %Ring=100%> increased = True
[13:36:07 INF] Starting publisher grain
[13:36:07 INF] Publisher started successfully on Sile S127.0.0.1:11111:83417762
[13:36:07 INF] Publisher grain started with Id 00000000-0000-0000-0000-000000000000
[13:36:09 INF] Starting subscriber grains
[13:36:10 INF] Subscriber started successfully on Sile S127.0.0.1:11111:83417762
[13:36:10 INF] Subscriber started successfully on Sile S127.0.0.1:11111:83417762
[13:36:10 INF] Subscriber started successfully on Sile S127.0.0.1:11111:83417762
[13:36:10 INF] 3 subscriber grains started
[13:36:14 INF] Subscriber Grain e3ccfd0618bc4383a7a7c2eab4c0d5cd on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:14 INF] Subscriber Grain e7c9aca209eb481daf0689a3940a7778 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:14 INF] Subscriber Grain c46a7d1b239345b5a77da236fbc35070 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:19 INF] Subscriber Grain e7c9aca209eb481daf0689a3940a7778 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:19 INF] Subscriber Grain e3ccfd0618bc4383a7a7c2eab4c0d5cd on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:19 INF] Subscriber Grain c46a7d1b239345b5a77da236fbc35070 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:19 INF] My range changed from <(0 0], Size=x100000000, %Ring=100%> to <MultiRange: Size=x7FF4EA1A, %Ring=49,983%> increased = True
[13:36:21 INF] Starting subscriber grains
[13:36:21 INF] 3 subscriber grains started
[13:36:24 DBG] Error during subscribing to publisher Grain
[13:36:24 DBG] Error during subscribing to publisher Grain
[13:36:24 DBG] Error during subscribing to publisher Grain
[13:36:24 INF] Subscriber Grain c46a7d1b239345b5a77da236fbc35070 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:24 INF] Subscriber Grain e7c9aca209eb481daf0689a3940a7778 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:24 INF] Subscriber Grain e3ccfd0618bc4383a7a7c2eab4c0d5cd on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:29 INF] Subscriber Grain e3ccfd0618bc4383a7a7c2eab4c0d5cd on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:29 INF] Subscriber Grain e7c9aca209eb481daf0689a3940a7778 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:29 INF] Subscriber Grain c46a7d1b239345b5a77da236fbc35070 on Silo S127.0.0.1:11111:83417762 received publication Message sent from publisher
[13:36:38 INF] My range changed from <MultiRange: Size=x7FF4EA1A, %Ring=49,983%> to <(0 0], Size=x100000000, %Ring=100%> increased = True
[13:36:38 INF] Subscriber stopped successfully
```

-----------------------------------------------------------------------------------------------------------------

### Sometimes the following exceptions are thrown:

Log of Silo2
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

Log of Silo1
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