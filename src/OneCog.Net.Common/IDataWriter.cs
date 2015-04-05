using System;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public interface IDataWriter : IDisposable
    {
        Task Write(byte[] bytes);
    }
}
