using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO.Ably;
using IO.Ably.AlternativeTransports.WebSockets4Net;

namespace AlternativeTransportExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var realtime =
                new AblyRealtime(new ClientOptions
                {
                    Key = "oFpaLg.RojibA:tyvnecXl8sjl2G6l",
                    TransportFactory = new WebSocket4NetTransport.WebSocketTransportFactory()
                });

            var channel = realtime.Channels.Get("alt-trans");

            channel.Subscribe(msg =>
                {
                    Console.WriteLine(msg.Data.ToString());
                });

            for (int i = 0; i < 10; i++)
            {
                channel.Publish("alt-transport", "using websocket4net");
            }

            Console.ReadLine();
        }
    }
}
