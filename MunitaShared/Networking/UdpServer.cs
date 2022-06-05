using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;

namespace Munita
{
    public class UdpServer : UdpBase
    {
        private IPEndPoint _listenOn;

        public UdpServer() : this(new IPEndPoint(IPAddress.Any, 7777))
        {
        }

        public UdpServer(IPEndPoint endpoint)
        {
            _listenOn = endpoint;
            Client = new System.Net.Sockets.UdpClient(_listenOn);

            Client.Client.IOControl(-1744830452, new byte[] { 0, 0, 0, 0 }, null);
        }

        public void Send(string code, string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes($"munitaClient777#{code}#{message}");
            Client.Send(datagram, datagram.Length, endpoint);
        }

        public void Reply(string code, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes($"munitaClient777#{code}");
            Client.Send(datagram, datagram.Length, endpoint);
        }
    }
}