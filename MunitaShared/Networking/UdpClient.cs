using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;

namespace Munita
{
    public class UdpClient : UdpBase
    {
        private UdpClient() {}

        public static UdpClient ConnectTo(string hostname, int port)
        {
            var connection = new UdpClient();
            connection.Client.Connect(hostname, port);
            
            return connection;
        }

        public void Send(string code, string username, string message)
        {
            var datagram = Encoding.ASCII.GetBytes($"munitaClient777#{code}#{username}#{message}");
            Client.Send(datagram, datagram.Length);
        }

        public void Reply(string code, string username)
        {
            var datagram = Encoding.ASCII.GetBytes($"munitaClient777#{code}#{username}");
            Client.Send(datagram, datagram.Length);
        }
    }
}