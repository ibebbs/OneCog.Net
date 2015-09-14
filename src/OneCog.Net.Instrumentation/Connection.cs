using System;
using System.Diagnostics.Tracing;

namespace OneCog.Net.Instrumentation
{
    [EventSource(Name = "OneCog-Net-Connection")]
    public sealed class Connection : EventSource
    {
        public static readonly Connection Log = new Connection();

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

        [Event(10, Message = "Start GetDataReader for {0}", Level = EventLevel.Informational, Opcode = EventOpcode.Start)]
        public void StartGetDataReader(string uri)
        {
            WriteEvent(10, uri);
        }

        [Event(11, Message = "Error getting data reader for {0}", Level = EventLevel.Error)]
        public void ErrorGettingDataReader(string uri, string exception)
        {
            WriteEvent(11, uri, exception);
        }

        [Event(12, Message = "Stop GetDataReader for {0}", Level = EventLevel.Informational, Opcode = EventOpcode.Stop)]
        public void StopGetDataReader(string uri)
        {
            WriteEvent(12, uri);
        }

        [Event(13, Message = "Start GetDataWriter for {0}", Level = EventLevel.Informational, Opcode = EventOpcode.Start)]
        public void StartGetDataWriter(string uri)
        {
            WriteEvent(13, uri);
        }

        [Event(14, Message = "Error getting data writer for {0}", Level = EventLevel.Error)]
        public void ErrorGettingDataWriter(string uri, string exception)
        {
            WriteEvent(14, uri, exception);
        }

        [Event(15, Message = "Stop GetDataWriter for {0}", Level = EventLevel.Informational, Opcode = EventOpcode.Stop)]
        public void StopGetDataWriter(string uri)
        {
            WriteEvent(15, uri);
        }

        [Event(16, Message = "Waiting for mutex before getting data reader", Level = EventLevel.Informational)]
        public void GetDataReaderWaitingForMutex()
        {
            WriteEvent(16);
        }

        [Event(17, Message = "Waiting for mutex before getting data writer", Level = EventLevel.Informational)]
        public void GetDataWriterWaitingForMutex()
        {
            WriteEvent(17);
        }
    }
}
