using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OneCog.Net
{
    public static class Util
    {
        public static IEnumerable<string> LocalHostNames()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Select(ip => ip.ToString());
        }
    }
}
