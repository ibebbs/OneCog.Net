using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using CoreReader = Windows.Storage.Streams.DataReader;
using CoreWriter = Windows.Storage.Streams.DataWriter;

namespace OneCog.Net
{
    public class TcpClient : ITcpClient
    {
        private Connection _connection;
        
        public async Task<IDisposable> Connect(Uri uri, CancellationToken cancellationToken)
        {
            if (_connection != null) throw new InvalidOperationException("Socket is already connected");

            Instrumentation.Connection.Log.ConnectingTo(uri.ToString());

            StreamSocket socket = new StreamSocket();

            Instrumentation.Connection.Log.OpeningConnection(uri.ToString());

            try
            {
                await socket.ConnectAsync(new HostName(uri.Host), uri.Port.ToString()).AsTask(cancellationToken);

                Instrumentation.Connection.Log.ConnectionOpened(uri.ToString());

                CoreReader dataReader = new CoreReader(socket.InputStream);
                CoreWriter dataWriter = new CoreWriter(socket.OutputStream);

                _connection = new Connection(socket, dataReader, dataWriter, () => _connection = null);

                return _connection;
            }
            catch (Exception e)
            {
                Instrumentation.Connection.Log.ConnectionFailed(uri.ToString(), e.ToString());

                throw;
            }
        }

        public Task<IDisposable> Connect(Uri uri)
        {
            return Connect(uri, CancellationToken.None);
        }

        public Task<IDisposable> Connect(string host, uint port, CancellationToken cancellationToken)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            return Connect(uri, cancellationToken);
        }

        public Task<IDisposable> Connect(string host, uint port)
        {
            Uri uri = new UriBuilder("tcp", host, (int)port).Uri;
            return Connect(uri, CancellationToken.None);
        }

        public Task<int> Read(byte[] bytes, CancellationToken cancellationToken)
        {
            if (_connection == null) throw new InvalidOperationException("No connection. Call Connect first");

            return _connection.Read(bytes, cancellationToken);
        }

        public Task Write(byte[] bytes, CancellationToken cancellationToken)
        {
            if (_connection == null) throw new InvalidOperationException("No connection. Call Connect first");

            return _connection.Write(bytes, cancellationToken);
        }
    }
}
