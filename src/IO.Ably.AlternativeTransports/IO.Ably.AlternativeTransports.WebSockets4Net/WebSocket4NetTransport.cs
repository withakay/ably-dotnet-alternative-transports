using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IO.Ably.Realtime;
using IO.Ably.Transport;
using WebSocket4Net;
using SuperSocket.ClientEngine;

namespace IO.Ably.AlternativeTransports.WebSockets4Net
{

    public class WebSocket4NetTransport : ITransport
    {
        public class WebSocketTransportFactory : ITransportFactory
        {
            public ITransport CreateTransport(TransportParams parameters)
            {
                return new WebSocket4NetTransport(parameters);
            }
        }

        private static readonly Dictionary<WebSocketState, TransportState> StateDict = new Dictionary
            <WebSocketState, TransportState>
                {
                    {WebSocketState.None, TransportState.Initialized},
                    {WebSocketState.Connecting, TransportState.Connecting},
                    {WebSocketState.Open, TransportState.Connected},
                    {WebSocketState.Closing, TransportState.Closing},
                    {WebSocketState.Closed, TransportState.Closed}
                };
        
        private WebSocket _socket;

        protected WebSocket4NetTransport(TransportParams parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters), "Null parameters are not allowed");

            BinaryProtocol = parameters.UseBinaryProtocol;
            WebSocketUri = parameters.GetUri();
        }

        public bool BinaryProtocol { get; }
        public Uri WebSocketUri { get; }

        public TransportState State
        {
            get
            {
                if (_socket == null)
                {
                    return TransportState.Initialized;
                }
                return StateDict[_socket.State];
            }
        }

        public ITransportListener Listener { get; set; }

        public void Connect()
        {
            if (_socket == null)
            {
                _socket = CreateSocket(WebSocketUri);
                AttachEvents();
            }
            
            _socket.Open();
        }

        public void Close(bool suppressClosedEvent = true)
        {
            if (_socket != null)
            {
                if (suppressClosedEvent)
                    DetachEvents();
                _socket.Close();
            }
        }

        public void Send(RealtimeTransportData data)
        {
            if (BinaryProtocol)
            {
                _socket.Send(data.Data, 0, data.Length);
            }
            else
            {
                _socket.Send(data.Text);
            }
        }

        private WebSocket CreateSocket(Uri uri)
        {
            return new WebSocket(uri.ToString(), "", WebSocketVersion.Rfc6455);
        }

        private void AttachEvents()
        {
            if (_socket != null)
            {
                _socket.Opened += socket_Opened;
                _socket.Closed += socket_Closed;
                _socket.Error += socket_Error;
                _socket.MessageReceived += socket_MessageReceived; //For text messages
                _socket.DataReceived += socket_DataReceived; //For binary messages    
            }
        }
        
        private void DetachEvents()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Opened -= socket_Opened;
                    _socket.Closed -= socket_Closed;
                    _socket.Error -= socket_Error;
                    _socket.MessageReceived -= socket_MessageReceived; //For text messages
                    _socket.DataReceived -= socket_DataReceived; //For binary messages    
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void socket_Opened(object sender, EventArgs e)
        {

            Listener?.OnTransportEvent(State);
        }

        private void socket_Closed(object sender, EventArgs e)
        {
            Listener?.OnTransportEvent(State);


            DetachEvents();
            _socket = null;
        }

        private void socket_Error(object sender, ErrorEventArgs e)
        {
            Listener?.OnTransportEvent(State, e.Exception);
        }

        private void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Listener?.OnTransportDataReceived(new RealtimeTransportData(e.Message));
        }

        private void socket_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Listener?.OnTransportDataReceived(new RealtimeTransportData(e.Data));
        }        

        public void Dispose()
        {
            if (_socket != null)
            {
                Close(true);
                _socket?.Dispose();
                _socket = null;
            }
        }
    }

}