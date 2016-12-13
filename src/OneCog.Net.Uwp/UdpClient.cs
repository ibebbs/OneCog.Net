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

        public async Task ConnectAsync(Uri localUri, Uri remoteUri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Udp.Log.ConnectingTo(remoteUri.ToString());

            DatagramSocket socket = new DatagramSocket();

            Instrumentation.Udp.Log.OpeningConnection(remoteUri.ToString());

            try
            {
                await socket.ConnectAsync(new EndpointPair(new HostName(localUri.Host), localUri.Port.ToString(), new HostName(remoteUri.Host), remoteUri.Port.ToString())).AsTask(cancellationToken);

                Instrumentation.Udp.Log.ConnectionOpened(remoteUri.ToString());

                CoreReader dataReader = new CoreReader(new DatagramSocketInputStream(socket));
                CoreWriter dataWriter = new CoreWriter(socket.OutputStream);

                _connection = new Connection(socket, dataReader, dataWriter, () => _connection = null);
            }
            catch (Exception e)
            {
                Instrumentation.Udp.Log.ConnectionFailed(remoteUri.ToString(), e.ToString());

                throw;
            }
        }

        public async Task ConnectAsync(Uri remoteUri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Udp.Log.ConnectingTo(remoteUri.ToString());

            DatagramSocket socket = new DatagramSocket();

            Instrumentation.Udp.Log.OpeningConnection(remoteUri.ToString());

            try
            {
                await socket.ConnectAsync(new HostName(remoteUri.Host), remoteUri.Port.ToString()).AsTask(cancellationToken);

                Instrumentation.Udp.Log.ConnectionOpened(remoteUri.ToString());
                
                CoreReader dataReader = new CoreReader(new DatagramSocketInputStream(socket));
                CoreWriter dataWriter = new CoreWriter(socket.OutputStream);

                _connection = new Connection(socket, dataReader, dataWriter, () => _connection = null);
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

        public Task ConnectAsync(string remoteHost, uint remotePort, CancellationToken cancellationToken)
        {
            Uri remoteUri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            return ConnectAsync(remoteUri, cancellationToken);
        }

        public Task ConnectAsync(string remoteHost, uint remotePort)
        {
            Uri remoteUri = new UriBuilder("udp", remoteHost, (int)remotePort).Uri;
            return ConnectAsync(remoteUri, CancellationToken.None);
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
