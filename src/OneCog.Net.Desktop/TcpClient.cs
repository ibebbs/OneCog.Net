using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public class TcpClient : ITcpClient
    {
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

            if (!_connections.TryGetValue(uri, out connection))
            {
                connection = new Connection(uri);
                _connections.Add(uri, connection);
            }

            return connection.GetDataReader();
        }

        public Task<IDataWriter> GetDataWriter(string host, uint port)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            Connection connection;

            if (!_connections.TryGetValue(uri, out connection))
            {
                connection = new Connection(uri);
                _connections.Add(uri, connection);
            }

            return connection.GetDataWriter();
        }
    }
}
