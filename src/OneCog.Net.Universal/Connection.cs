using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using CoreReader = Windows.Storage.Streams.DataReader;
using CoreWriter = Windows.Storage.Streams.DataWriter;

namespace OneCog.Net
{
    internal class Connection : IDisposable
    {
        private StreamSocket _streamSocket;
        private CoreReader _dataReader;
        private CoreWriter _dataWriter;
        private Action _disposed;

        public Connection(StreamSocket streamSocket, CoreReader dataReader, CoreWriter dataWriter, Action disposed)
        {
            _streamSocket = streamSocket;
            _dataReader = dataReader;
            _dataWriter = dataWriter;
            _disposed = disposed;
        }

        public void Dispose()
        {
            if (_dataReader != null)
            {
                _dataReader.Dispose();
                _dataReader = null;
            }

            if (_dataWriter != null)
            {
                _dataWriter.Dispose();
                _dataWriter = null;
            }

            if (_streamSocket != null)
            {
                _streamSocket.Dispose();
                _streamSocket = null;
            }

            if (_disposed != null)
            {
                _disposed();
                _disposed = null;
            }
        }

        public async Task<int> Read(byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                uint loaded = await _dataReader.LoadAsync((uint)bytes.Length).AsTask(cancellationToken);
                _dataReader.ReadBytes(bytes);

                return (int)loaded;
            }
            catch (TaskCanceledException)
            {
                // Disposing so do nothing
                return -1;
            }
        }

        public async Task Write(byte[] bytes, CancellationToken cancellationToken)
        {
            try
            {
                _dataWriter.WriteBytes(bytes);
                await _dataWriter.StoreAsync().AsTask(cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // Disposing so do nothing
            }
        }
    }
}
