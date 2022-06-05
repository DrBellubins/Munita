using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;

namespace Munita
{
    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
    }

    public abstract class UdpBase
    {
        public System.Net.Sockets.UdpClient Client;

        protected UdpBase()
        {
            Client = new System.Net.Sockets.UdpClient();
        }

        public async Task<Received> Receive()
        {
            var result = await Client.ReceiveAsync();

            return new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer),
                Sender = result.RemoteEndPoint
            };
        }
    }
}