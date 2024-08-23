Demonstrates crashing / not working orleans streams after one silo shutdown.

Urls:
 - http://localhost:5000/dashboard
 - http://localhost:5000/pub/{intervalInSeconds}
 - http://localhost:5000/sub/{numOfSubs}

POC steps:
1. Start Silo1 and call following URLs, http://localhost:5000/pub/5, http://localhost:5000/sub/2
2. Start Silo2 and call following URL: http://localhost:5000/sub/2
3. Check log of Silo1 and Silo2, every Silo should receive messages from publisher every 5 seconds
4. Stop or kill Silo2
5. Check log of Silo1, all subscribers are stopped and no messages are received from publisher anymore!