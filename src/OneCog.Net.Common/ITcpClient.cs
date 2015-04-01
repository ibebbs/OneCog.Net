using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public interface ITcpClient : IDisposable
    {
        Task Connect(string host, uint port);
        Task Read(byte[] bytes);
        Task Write(byte[] bytes);
    }
}
