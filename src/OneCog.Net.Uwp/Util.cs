using System.Collections.Generic;
using System.Linq;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace OneCog.Net
{
    public static class Util
    {
        public static IEnumerable<string> LocalHostNames()
        {
            return NetworkInformation.GetHostNames().Where(localHostName => localHostName.IPInformation != null && localHostName.Type == HostNameType.Ipv4).Select(localHostName => localHostName.ToString());
        }
    }
}
