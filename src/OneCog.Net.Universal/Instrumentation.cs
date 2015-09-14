using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Net
{
    [EventSource(Name = "OneCog-Net")]
    sealed class Instrumentation : EventSource
    {
        public static Instrumentation Log = new Instrumentation();

        [Event(1, Message = "Connecting to {0}", Level = EventLevel.Informational)]
        public void ConnectingTo(Uri uri)
        {
            WriteEvent(1, uri);
        }

        [Event(2, Message = "Aborting connection: connection to {0} already active", Level = EventLevel.Warning)]
        public void AbortConnection(Uri uri)
        {
            WriteEvent(2, uri);
        }

        [Event(3, Message = "Opening connection to {0}", Level = EventLevel.Informational)]
        public void OpeningConnection(Uri uri)
        {
            WriteEvent(3, uri);
        }

        [Event(4, Message = "Connection to {0} opened successfully", Level = EventLevel.Informational)]
        public void ConnectionOpened(Uri uri)
        {
            WriteEvent(4, uri);
        }

        [Event(5, Message = "Connection to {0} failed with exception {1}", Level = EventLevel.Error)]
        public void ConnectionFailed(Uri uri, Exception exception)
        {
            WriteEvent(5, uri, exception);
        }
    }
}
