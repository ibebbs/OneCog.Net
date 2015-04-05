using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreReader = Windows.Storage.Streams.DataReader;

namespace OneCog.Net
{
    internal class DataReader : IDataReader
    {
        private readonly Connection _connection;
        private CoreReader _reader;

        public DataReader(Connection connection)
        {
            _connection = connection;
            _connection.AddReader(this);

            _reader = new CoreReader(_connection.StreamSocket.InputStream);
        }

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }

            _connection.RemoveReader(this);
        }

        public async Task Read(byte[] bytes)
        {
            await _reader.LoadAsync((uint)bytes.Length);
            _reader.ReadBytes(bytes);
        }
    }
}
