# Alternative Transports for ably-dotnet

## Websocket4net

Pass an instance of ITransportFactory into your ClientOptions 

```csharp
var realtime = new AblyRealtime(new ClientOptions { TransportFactory = new  IO.Ably.AlternativeTransports.WebSocket4NetTransport.WebSocketTransportFactory() });
```
