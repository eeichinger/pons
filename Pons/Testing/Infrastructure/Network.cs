using System;
using System.Net;
using System.Net.Sockets;

namespace Viz.Testing
{
    public static class Network
    {
        public static int GetNextAvailableTcpPort()
        {
            return GetNextAvailableIpPort(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static int GetNextAvailableIpPort(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            for (int port = 2000; port < 65535; port++)
            {
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
                Socket socket = new Socket(addressFamily, socketType, protocolType);

                try
                {
                    socket.Bind(ep);
                    socket.Close();
                    //Port available
                    return port;
                }
                catch (SocketException)
                {
//                    Debug.WriteLine(string.Format("Port not available {0}", port));
                }
            }            
            throw new SystemException("could not find available port");
        }

        public static IPAddress GetNonLocalIPv4Address()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostName);
            foreach (IPAddress addr in host.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    string strAddr = addr.ToString();
                    if (!strAddr.Equals("127.0.0.1"))
                    {
                        return addr;
                    }
                }
            }
            throw new SystemException("does not exist");
        }
    }
}
