using System;
using System.Diagnostics.Tracing;

namespace OneCog.Net.Instrumentation
{
    [EventSource(Name = "OneCog-Net-Udp")]
    public sealed class Udp : EventSource
    {
        public static readonly Udp Log = new Udp();

        [Event(1, Message = "Connecting to {0}", Level = EventLevel.Informational)]
        public void ConnectingTo(string uri)
        {
            WriteEvent(1, uri);
        }

        [Event(2, Message = "Aborting connection: connection to {0} already active", Level = EventLevel.Warning)]
        public void AbortConnection(string uri)
        {
            WriteEvent(2, uri);
        }

        [Event(3, Message = "Opening connection to {0}", Level = EventLevel.Informational)]
        public void OpeningConnection(string uri)
        {
            WriteEvent(3, uri);
        }

        [Event(4, Message = "Connection to {0} opened successfully", Level = EventLevel.Informational)]
        public void ConnectionOpened(string uri)
        {
            WriteEvent(4, uri);
        }

        [Event(5, Message = "Connection to {0} failed with exception {1}", Level = EventLevel.Error)]
        public void ConnectionFailed(string uri, string exception)
        {
            WriteEvent(5, uri, exception);
        }

        [Event(6, Message = "Disconnecting from {0}", Level = EventLevel.Informational)]
        public void DisconnectingFrom(string uri)
        {
            WriteEvent(6, uri);
        }

        [Event(7, Message = "Disposing connection to {0}", Level = EventLevel.Informational)]
        public void DisposingConnection(string uri)
        {
            WriteEvent(7, uri);
        }

        [Event(8, Message = "Connection to {0} disposed successfully", Level = EventLevel.Informational)]
        public void ConnectionDisposed(string uri)
        {
            WriteEvent(8, uri);
        }

        [Event(9, Message = "Aborting disconnection: connection to {0} already disposed", Level = EventLevel.Warning)]
        public void AbortDisconnection(string uri)
        {
            WriteEvent(9, uri);
        }
    }
}
