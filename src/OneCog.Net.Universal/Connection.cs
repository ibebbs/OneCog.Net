using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace OneCog.Net
{
    internal class Connection : IDisposable
    {
        private readonly List<IDisposable> _consumers;

        public Connection(Uri uri)
        {
            _consumers = new List<IDisposable>();

            Uri = uri;
        }

        public void Dispose()
        {
            IEnumerable<IDisposable> consumers = _consumers.ToArray();

            foreach (IDisposable consumer in consumers)
            {
                consumer.Dispose();
            }
        }

        private async Task Connect()
        {
            Instrumentation.Connection.Log.ConnectingTo(Uri.ToString());

            if (StreamSocket == null)
            {
                StreamSocket = new StreamSocket();

                Instrumentation.Connection.Log.OpeningConnection(Uri.ToString());

                try
                {
                    await StreamSocket.ConnectAsync(new HostName(Uri.Host), Uri.Port.ToString());

                    Instrumentation.Connection.Log.ConnectionOpened(Uri.ToString());
                }
                catch (Exception e)
                {
                    Instrumentation.Connection.Log.ConnectionFailed(Uri.ToString(), e.ToString());

                    throw;
                }
            }
            else
            {
                Instrumentation.Connection.Log.AbortConnection(Uri.ToString());
            }
        }

        private void Disconnect()
        {
            Instrumentation.Connection.Log.DisconnectingFrom(Uri.ToString());

            if (StreamSocket != null)
            {
                Instrumentation.Connection.Log.DisposingConnection(Uri.ToString());
                StreamSocket.Dispose();
                StreamSocket = null;
                Instrumentation.Connection.Log.ConnectionDisposed(Uri.ToString());
            }
            else
            {
                Instrumentation.Connection.Log.AbortDisconnection(Uri.ToString());
            }
        }

        public void AddReader(DataReader dataReader)
        {
            _consumers.Add(dataReader);
        }

        public void RemoveReader(DataReader dataReader)
        {
            _consumers.Remove(dataReader);

            if (_consumers.Count == 0)
            {
                Disconnect();
            }
        }

        public void AddWriter(DataWriter dataWriter)
        {
            _consumers.Add(dataWriter);
        }

        public void RemoveWriter(DataWriter dataWriter)
        {
            _consumers.Remove(dataWriter);

            if (_consumers.Count == 0)
            {
                Disconnect();
            }
        }

        public async Task<IDataReader> GetDataReader()
        {
            await Connect();

            return new DataReader(this);
        }

        public async Task<IDataWriter> GetDataWriter()
        {
            await Connect();

            return new DataWriter(this);
        }

        public Uri Uri { get; private set; }

        public StreamSocket StreamSocket { get; private set; }
    }
}
