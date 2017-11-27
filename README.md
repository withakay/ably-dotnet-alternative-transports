# Alternative Transports for ably-dotnet

## Websocket4net

Pass an instance of ITransport factory into the ClientOptions 

`var realtime = new AblyRealtime(new ClientOptions { TransportFactory = new  IO.Ably.AlternativeTransports.WebSocket4NetTransport.WebSocketTransportFactory() });`
