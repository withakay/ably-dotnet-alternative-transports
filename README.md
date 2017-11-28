# Alternative Transports for ably-dotnet

It is possible to create alternate websockets transports for use with [ably-dotnet](https://github.com/ably/ably-dotnet). 
This can be required to allow ably-dotnet to work on older platforms, such as windows 7.

## Websocket4net

This example uses the Webscoket4net library.
To add this code to your project either clone this repository and add it to your solution and reference it from your project or simply copy the [WebSocket4NetTransport.cs](https://github.com/withakay/ably-dotnet-alternative-transports/blob/master/src/IO.Ably.AlternativeTransports/IO.Ably.AlternativeTransports.WebSockets4Net/WebSocket4NetTransport.cs) file into your project and added the relevant Ably.io and WebSocket4net nuget references.

Once you have the done that you just need to pass an instance of ITransportFactory into your ClientOptions like so:

```csharp
var realtime = 
  new AblyRealtime(new ClientOptions { 
    TransportFactory = new WebSocket4NetTransport.WebSocketTransportFactory() 
  });
```
