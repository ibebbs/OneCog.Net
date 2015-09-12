using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using CoreReader = Windows.Storage.Streams.DataReader;

namespace OneCog.Net
{
    internal class DataReader : IDataReader
    {
        private readonly Connection _connection;

        private CancellationTokenSource _cancellationTokenSource;
        private CoreReader _reader;

        public DataReader(Connection connection)
        {
            _connection = connection;
            _connection.AddReader(this);

            _cancellationTokenSource = new CancellationTokenSource();
            
            _reader = new CoreReader(_connection.StreamSocket.InputStream);
        }

        public void Dispose()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            _connection.RemoveReader(this);
        }

        public async Task Read(byte[] bytes)
        {
            Task<uint> load = _reader.LoadAsync((uint)bytes.Length).AsTask(_cancellationTokenSource.Token);

            await load;

            if (load.Status == TaskStatus.RanToCompletion)
            {
                _reader.ReadBytes(bytes);
            }
        }
    }
}
