using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Net
{
    public interface IDataReader : IDisposable
    {
        Task Read(byte[] bytes);
    }
}
