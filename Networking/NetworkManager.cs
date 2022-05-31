using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;
using System.Net.Sockets;

using Raylib_cs;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Munita
{
    public class ClientListener : INetEventListener
    {
        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Announce($"[Client] connected to: {peer.EndPoint.Address}:{peer.EndPoint.Port}");

            /*NetDataWriter dataWriter = new NetDataWriter();

            dataWriter.Reset();
            dataWriter.Put(TestClientVec.X);
            dataWriter.Put(TestClientVec.Y);
            peer.Send(dataWriter, DeliveryMethod.ReliableOrdered);*/
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Warning($"[Client] disconnected: {disconnectInfo.Reason}");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Debug.Error($"[Client] error! {socketErrorCode}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            var pos = new Vector2(reader.GetFloat(), reader.GetFloat());
            ClientPlayer.NetUpdate(pos);

            //Debug.Log($"[Client] received: {pos.X}, {pos.Y}");
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
        }
    }

    public class ServerListener : INetEventListener
    {
        public NetManager Server;

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Announce($"[Server] Peer connected: {peer.EndPoint}");

            foreach (var netPeer in Server)
            {
                if (netPeer.ConnectionState == ConnectionState.Connected)
                    Debug.Log($"ConnectedPeersList: id={netPeer.Id}, ep={netPeer.EndPoint}");
            }
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Warning($"[Server] Peer disconnected: {peer.EndPoint}, reason: {disconnectInfo.Reason}");
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Debug.Error($"[Server] error: {socketErrorCode}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            ServerPlayer.NetUpdate(reader.GetBool(), 
                new Vector2(reader.GetFloat(), reader.GetFloat()));
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Debug.Warning($"[Server] ReceiveUnconnected: {reader.GetString(100)}");
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var acceptedPeer = request.AcceptIfKey("munita-client777");

            Debug.Log($"[Server] ConnectionRequest. Ep: {request.RemoteEndPoint}, Accepted: {acceptedPeer != null}");
        }
    }
}