using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreWriter = Windows.Storage.Streams.DataWriter;

namespace OneCog.Net
{
    internal class DataWriter : IDataWriter
    {
        private readonly Connection _connection;
        private CoreWriter _writer;

        public DataWriter(Connection connection)
        {
            _connection = connection;
            _connection.AddWriter(this);

            _writer = new CoreWriter(_connection.StreamSocket.OutputStream);
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }

            _connection.RemoveWriter(this);
        }

        public async Task Write(byte[] bytes)
        {
            _writer.WriteBytes(bytes);
            await _writer.StoreAsync();
        }
    }
}
