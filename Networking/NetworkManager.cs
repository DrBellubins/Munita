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
    // TODO: When leaving and rejoining server doesn't send pos to client
    public class MunitaServer : INetEventListener
    {
        public NetManager UdpServer;
        public List<ServerPlayer> Players = new List<ServerPlayer>();

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Announce($"[Server] Peer connected: {peer.EndPoint}");

            if (peer.ConnectionState == ConnectionState.Connected)
                Debug.Log($"ConnectedPeersList: id = {peer.Id}, ep = {peer.EndPoint}");
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
            var username = reader.GetString();

            var player = GetPlayerByUsername(username);

            if (player != null)
            {
                //player.Peer = peer; // TEMP: DISABLES MORE THAN 1 PLAYER
                player.IsRunning = reader.GetBool();
                player.MoveDirection = new Vector2(reader.GetFloat(), reader.GetFloat());
            }
            else
            {
                player = new ServerPlayer();
                player.Initialize(peer);

                player.Username = username;

                Players.Add(player);
            }
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

        public ServerPlayer? GetPlayerByUsername(string username)
        {
            return Players.Where(x => x.Username == username).FirstOrDefault();
        }
    }

    public class MunitaClient : INetEventListener
    {
        public void LoadConfig()
        {
            var configPath = $"{Environment.CurrentDirectory}\\client.conf";

            if (File.Exists(configPath))
            {
                Debug.Warning($"Client.conf already exists, loading.");

                var configLines = File.ReadAllLines(configPath);

                var usernameLineStr = configLines[0];
                var usernameLineParseStr = usernameLineStr.Split("Username: ", 2, StringSplitOptions.RemoveEmptyEntries);

                ClientPlayer.Username = usernameLineParseStr[0];

                var ipLineStr = configLines[1];
                var ipLineParseStr = ipLineStr.Split("IP: ", 2, StringSplitOptions.RemoveEmptyEntries);

                ClientEngine.ServerIP = ipLineParseStr[0];
            }
            else
            {
                using (var fs = new FileStream(configPath, FileMode.CreateNew))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine("Username: Player");
                        sw.WriteLine("IP: 127.0.0.1");
                    }
                }
            }
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Announce($"[Client] connected to: {peer.EndPoint.Address}:{peer.EndPoint.Port}");
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
            //var playerCount = reader.GetInt();
            var currentPlayerPos = new Vector2(reader.GetFloat(), reader.GetFloat());

            ClientPlayer.NetUpdate(currentPlayerPos);

            /*ClientEngine.PlayerPositions.Clear();

            for (int i = 1; i < playerCount; i++)
            {
                ClientEngine.PlayerPositions.Add(new Vector2(reader.GetFloat(), reader.GetFloat()));
            }*/
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
}