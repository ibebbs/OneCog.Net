using System;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public interface ITcpClient : IDisposable
    {
        Task<IDataReader> GetDataReader(string host, uint port);
        Task<IDataWriter> GetDataWriter(string host, uint port);
    }
}
