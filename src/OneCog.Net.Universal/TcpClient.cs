using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace OneCog.Net
{
    public class TcpClient : ITcpClient
    {
        private readonly object _syncRoot = new object();
        private readonly Dictionary<Uri, Connection> _connections;

        public TcpClient()
        {
            _connections = new Dictionary<Uri, Connection>();
        }

        public void Dispose()
        {
            IEnumerable<Connection> connections = _connections.Values.ToArray();

            foreach (Connection connection in connections)
            {
                connection.Dispose();
            }
        }

        public Task<IDataReader> GetDataReader(string host, uint port)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            Connection connection;

            lock (_syncRoot)
            {
                if (!_connections.TryGetValue(uri, out connection))
                {
                    connection = new Connection(uri);
                    _connections.Add(uri, connection);
                }

                return connection.GetDataReader();
            }
        }

        public Task<IDataWriter> GetDataWriter(string host, uint port)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            Connection connection;

            lock (_syncRoot)
            {
                if (!_connections.TryGetValue(uri, out connection))
                {
                    connection = new Connection(uri);
                    _connections.Add(uri, connection);
                }

                return connection.GetDataWriter();
            }
        }
    }
}
