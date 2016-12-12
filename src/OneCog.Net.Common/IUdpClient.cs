using System;
using System.Threading;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public interface IUdpClient : IDisposable
    {
        Task ConnectAsync(Uri uri);

        Task ConnectAsync(string host, uint port);

        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        Task ConnectAsync(string host, uint port, CancellationToken cancellationToken);

        Task<int> ReadAsync(byte[] bytes, CancellationToken cancellationToken);

        Task WriteAsync(byte[] bytes, CancellationToken cancellationToken);
    }
}
