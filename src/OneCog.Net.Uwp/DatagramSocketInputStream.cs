using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using StreamBuffer = Windows.Storage.Streams.Buffer;

namespace OneCog.Net
{
    [ComImport]
    [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public class DatagramSocketInputStream : IInputStream
    {
        static unsafe void Copy(byte[] source, int sourceOffset, byte* target, int targetOffset, int count)
        {
            // If either array is not instantiated, you cannot complete the copy.
            if ((source == null) || (target == null))
            {
                throw new ArgumentException();
            }

            // If either offset, or the number of bytes to copy, is negative, you
            // cannot complete the copy.
            if ((sourceOffset < 0) || (targetOffset < 0) || (count < 0))
            {
                throw new ArgumentException();
            }

            // If the number of bytes from the offset to the end of the array is 
            // less than the number of bytes you want to copy, you cannot complete
            // the copy. 
            if (source.Length - sourceOffset < count)
            {
                throw new ArgumentException();
            }

            // The following fixed statement pins the location of the source and
            // target objects in memory so that they will not be moved by garbage
            // collection.
            fixed (byte* pSource = source)
            {
                // Set the starting points in source and target for the copying.
                byte* ps = pSource + sourceOffset;
                byte* pt = target + targetOffset;

                // Copy the specified number of bytes from source to target.
                for (int i = 0; i < count; i++)
                {
                    *pt = *ps;
                    pt++;
                    ps++;
                }
            }
        }

        private readonly DatagramSocket _socket;
        private object _lock;
        private byte[] _readBytes;

        private AutoResetEvent _waitHandle;

        public DatagramSocketInputStream(DatagramSocket socket)
        {
            _socket = socket;
            _lock = new object();
            _readBytes = new byte[0];

            _waitHandle = new AutoResetEvent(false);

            socket.MessageReceived += Socket_MessageReceived;
        }

        private void Socket_MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {            
            using (DataReader dataReader = args.GetDataReader())
            {
                while (dataReader.UnconsumedBufferLength > 0)
                {
                    byte[] buffer = new byte[dataReader.UnconsumedBufferLength];
                    dataReader.ReadBytes(buffer);

                    lock (_lock)
                    {
                        _readBytes = _readBytes.Concat(buffer).ToArray();
                    }

                    _waitHandle.Set();
                }
            }
        }

        public void Dispose()
        {
            _socket.MessageReceived -= Socket_MessageReceived;
            _waitHandle.Dispose();
        }

        public unsafe IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return AsyncInfo.Run<IBuffer, uint>((ct, progress) => Task.Run(() =>
                {
                    int bytesRead = 0;

                    while (true)
                    {
                        if (_waitHandle.WaitOne(TimeSpan.FromMilliseconds(100)))
                        {
                            using (MemoryBuffer memoryBuffer = StreamBuffer.CreateMemoryBufferOverIBuffer(buffer))
                            {
                                IMemoryBufferByteAccess reference = memoryBuffer.CreateReference() as IMemoryBufferByteAccess;

                                byte* target;
                                uint capacity;

                                reference.GetBuffer(out target, out capacity);

                                int maxBytesToCopy = (int) Math.Min(count, capacity);
                                int actualBytesToCopy;

                                lock (_lock)
                                {
                                    actualBytesToCopy = Math.Min(maxBytesToCopy, _readBytes.Length);

                                    Copy(_readBytes, 0, target, bytesRead, actualBytesToCopy);

                                    _readBytes = _readBytes.Skip(maxBytesToCopy).ToArray();
                                }

                                bytesRead += actualBytesToCopy;

                                if (options == InputStreamOptions.Partial && bytesRead > 0 || bytesRead == count)
                                {
                                    return buffer;
                                }
                                else
                                {
                                    progress.Report((uint)bytesRead);
                                }
                            }
                        }

                        ct.ThrowIfCancellationRequested();
                    }
                }
            ));
        }
    }
}
