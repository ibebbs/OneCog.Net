using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using CoreReader = Windows.Storage.Streams.DataReader;
using CoreWriter = Windows.Storage.Streams.DataWriter;

namespace OneCog.Net
{
    public class UdpClient : IUdpClient
    {
        private Connection _connection;

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Tcp.Log.ConnectingTo(uri.ToString());

            DatagramSocket socket = new DatagramSocket();

            Instrumentation.Udp.Log.OpeningConnection(uri.ToString());

            try
            {
                await socket.ConnectAsync(new HostName(uri.Host), uri.Port.ToString()).AsTask(cancellationToken);

                Instrumentation.Tcp.Log.ConnectionOpened(uri.ToString());
                
                CoreReader dataReader = new CoreReader(new DatagramSocketInputStream(socket));
                CoreWriter dataWriter = new CoreWriter(socket.OutputStream);

                _connection = new Connection(socket, dataReader, dataWriter, () => _connection = null);
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
