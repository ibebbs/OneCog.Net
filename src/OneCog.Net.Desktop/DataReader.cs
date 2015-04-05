using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Net
{
    internal class DataReader : IDataReader
    {
        private readonly Connection _connection;

        public DataReader(Connection connection)
        {
            _connection = connection;
            _connection.AddReader(this);
        }

        public void Dispose()
        {
            _connection.RemoveReader(this);
        }

        public Task Read(byte[] bytes)
        {
            return _connection.NetworkStream.ReadAsync(bytes, 0, bytes.Length);
        }
    }
}
