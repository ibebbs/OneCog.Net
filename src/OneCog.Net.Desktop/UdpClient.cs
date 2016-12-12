using System;
using System.Threading;
using System.Threading.Tasks;
using CoreUdpClient = System.Net.Sockets.UdpClient;

namespace OneCog.Net
{
    public class UdpClient : IUdpClient
    {
        private UdpConnection _connection;

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Tcp.Log.ConnectingTo(uri.ToString());

            CoreUdpClient socket = new CoreUdpClient();

            Instrumentation.Tcp.Log.OpeningConnection(uri.ToString());

            try
            {
                socket.Connect(uri.Host, uri.Port);

                Instrumentation.Tcp.Log.ConnectionOpened(uri.ToString());

                _connection = new UdpConnection(socket, () => _connection = null);

                return TaskEx.Completed;
            }
            catch (Exception e)
            {
                Instrumentation.Tcp.Log.ConnectionFailed(uri.ToString(), e.ToString());

                throw;
            }
        }

        public Task ConnectAsync(Uri uri)
        {
            return ConnectAsync(uri, CancellationToken.None);
        }

        public Task ConnectAsync(string host, uint port, CancellationToken cancellationToken)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            return ConnectAsync(uri, cancellationToken);
        }

        public Task ConnectAsync(string host, uint port)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            return ConnectAsync(uri, CancellationToken.None);
        }

        public Task<int> ReadAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            if (_connection == null) throw new InvalidOperationException("No connection. Call Connect first");

            return _connection.Read(bytes, cancellationToken);
        }

        public Task WriteAsync(byte[] bytes, CancellationToken cancellationToken)
        {
            if (_connection == null) throw new InvalidOperationException("No connection. Call Connect first");

            return _connection.Write(bytes, cancellationToken);
        }
    }
}
