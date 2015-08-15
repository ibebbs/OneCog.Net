using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CoreTcpClient = System.Net.Sockets.TcpClient;

namespace OneCog.Net
{
    internal class Connection : IDisposable
    {
        private CoreTcpClient _tcpClient;
        private readonly List<IDisposable> _consumers;

        public Connection(Uri uri)
        {
            _tcpClient = new CoreTcpClient();
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
            if (NetworkStream == null)
            {
                await _tcpClient.ConnectAsync(Uri.Host, Uri.Port);
                NetworkStream = _tcpClient.GetStream();
            }
        }

        private void Disconnect()
        {
            if (NetworkStream != null)
            {
                NetworkStream.Dispose();
                NetworkStream = null;

                _tcpClient.Close();
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

        public NetworkStream NetworkStream { get; private set; }
    }
}
