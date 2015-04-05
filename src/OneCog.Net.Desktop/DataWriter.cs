using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Net
{
    internal class DataWriter : IDataWriter
    {
        private readonly Connection _connection;

        public DataWriter(Connection connection)
        {
            _connection = connection;
            _connection.AddWriter(this);
        }

        public void Dispose()
        {
            _connection.RemoveWriter(this);
        }

        public Task Write(byte[] bytes)
        {
            return _connection.NetworkStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
