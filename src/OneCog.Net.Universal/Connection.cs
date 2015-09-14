﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace OneCog.Net
{
    internal class Connection : IDisposable
    {
        private readonly List<IDisposable> _consumers;

        public Connection(Uri uri)
        {
            _consumers = new List<IDisposable>();

            Uri = uri;
        }

        public void Dispose()
        {
            IEnumerable<IDisposable> consumers = _consumers.ToArray();

            foreach (IDisposable consumer in consumers)
            {
                consumer.Dispose();
            }
        }

        private async Task Connect()
        {
            Instrumentation.Log.ConnectingTo(Uri);

            if (StreamSocket == null)
            {
                StreamSocket = new StreamSocket();

                Instrumentation.Log.OpeningConnection(Uri);

                try
                {
                    await StreamSocket.ConnectAsync(new HostName(Uri.Host), Uri.Port.ToString());

                    Instrumentation.Log.ConnectionOpened(Uri);
                }
                catch (Exception e)
                {
                    Instrumentation.Log.ConnectionFailed(Uri, e);

                    throw;
                }
            }
            else
            {
                Instrumentation.Log.AbortConnection(Uri);
            }
        }

        private void Disconnect()
        {
            if (StreamSocket != null)
            {
                StreamSocket.Dispose();
                StreamSocket = null;
            }
        }

        public void AddReader(DataReader dataReader)
        {
            _consumers.Add(dataReader);
        }

        public void RemoveReader(DataReader dataReader)
        {
            _consumers.Remove(dataReader);

            if (_consumers.Count == 0)
            {
                Disconnect();
            }
        }

        public void AddWriter(DataWriter dataWriter)
        {
            _consumers.Add(dataWriter);
        }

        public void RemoveWriter(DataWriter dataWriter)
        {
            _consumers.Remove(dataWriter);

            if (_consumers.Count == 0)
            {
                Disconnect();
            }
        }

        public async Task<IDataReader> GetDataReader()
        {
            await Connect();

            return new DataReader(this);
        }

        public async Task<IDataWriter> GetDataWriter()
        {
            await Connect();

            return new DataWriter(this);
        }

        public Uri Uri { get; private set; }

        public StreamSocket StreamSocket { get; private set; }
    }
}
