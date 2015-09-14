using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public interface ITcpClient
    {
        Task<IDisposable> Connect(Uri uri);

        Task<IDisposable> Connect(string host, uint port);

        Task<IDisposable> Connect(Uri uri, CancellationToken cancellationToken);

        Task<IDisposable> Connect(string host, uint port, CancellationToken cancellationToken);

        Task<int> Read(byte[] bytes, CancellationToken cancellationToken);

        Task Write(byte[] bytes, CancellationToken cancellationToken);
    }
}
