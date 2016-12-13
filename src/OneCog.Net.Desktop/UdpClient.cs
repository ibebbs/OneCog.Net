using System;
using System.Net;
using System.Net.Sockets;
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

        public Task ConnectAsync(Uri localUri, Uri remoteUri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Udp.Log.ConnectingTo(remoteUri.ToString());

            CoreUdpClient socket = new CoreUdpClient();

            socket.Client.ExclusiveAddressUse = false;
            socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Client.Bind(new IPEndPoint(IPAddress.Parse(localUri.Host), localUri.Port));

            Instrumentation.Udp.Log.OpeningConnection(remoteUri.ToString());

            try
            {
                socket.Connect(remoteUri.Host, remoteUri.Port);

                Instrumentation.Udp.Log.ConnectionOpened(remoteUri.ToString());

                _connection = new UdpConnection(socket, () => _connection = null);

                return TaskEx.Completed;
            }
            catch (Exception e)
            {
                Instrumentation.Udp.Log.ConnectionFailed(remoteUri.ToString(), e.ToString());

                throw;
            }
        }

        public Task ConnectAsync(Uri remoteUri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Udp.Log.ConnectingTo(remoteUri.ToString());

            CoreUdpClient socket = new CoreUdpClient();

            Instrumentation.Udp.Log.OpeningConnection(remoteUri.ToString());

            try
            {
                socket.Connect(remoteUri.Host, remoteUri.Port);

                Instrumentation.Udp.Log.ConnectionOpened(remoteUri.ToString());

                _connection = new UdpConnection(socket, () => _connection = null);

                return TaskEx.Completed;
            }
            catch (Exception e)
            {
                Instrumentation.Udp.Log.ConnectionFailed(remoteUri.ToString(), e.ToString());

                throw;
            }
        }

        public Task ConnectAsync(Uri remoteUri)
        {
            return ConnectAsync(remoteUri, CancellationToken.None);
        }

        public Task ConnectAsync(Uri localUri, Uri remoteUri)
        {
            return ConnectAsync(localUri, remoteUri, CancellationToken.None);
        }

        public Task ConnectAsync(string remoteHost, uint remotePort, CancellationToken cancellationToken)
        {
            Uri uri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            return ConnectAsync(uri, cancellationToken);
        }

        public Task ConnectAsync(string remoteHost, uint remotePort)
        {
            Uri uri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            return ConnectAsync(uri, CancellationToken.None);
        }

        public Task ConnectAsync(string localHost, uint localPort, string remoteHost, uint remotePort, CancellationToken cancellationToken)
        {
            Uri localUri = new UriBuilder("udp", localHost, (int)localPort).Uri;
            Uri remoteUri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            return ConnectAsync(localUri, remoteUri, cancellationToken);
        }

        public Task ConnectAsync(string localHost, uint localPort, string remoteHost, uint remotePort)
        {
            Uri localUri = new UriBuilder("udp", localHost, (int)localPort).Uri;
            Uri remoteUri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            Uri uri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            return ConnectAsync(localUri, remoteUri, CancellationToken.None);
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
