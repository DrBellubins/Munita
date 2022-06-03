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
    public class ServerEngine
    {
        public const int TicksPerSecond = 20;
        public const int TickRate = 1000 / TicksPerSecond;

        public bool IsRunning;
        public bool IsPaused;

        public void Initialize()
        {
            Debug.Announce("Server engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Networking
            var udpListener = new MunitaServer();
            var udpServer = new NetManager(udpListener);

            udpListener.UdpServer = udpServer;

            udpServer.ReuseAddress = true;

            udpServer.Start(25565);

            // Initialize
            var world = new World();
            world.Initialize(false);

            while (IsRunning)
            {
                currentTimer = DateTime.Now;
                
                // Update
                udpServer.PollEvents();

                world.Update();

                if (udpServer.ConnectedPeersCount > 0)
                {
                    for (int i = 0; i < udpListener.Players.Count; i++)
                    {
                        var player = udpListener.GetPlayerByUsername(udpListener.Players[i].Username);

                        if (player != null)
                        {
                            player.Update(deltaTime);

                            // Packet order:
                            // Player count
                            // Current player position
                            // Other player positions
                            var dataWriter = new NetDataWriter();
                            //dataWriter.Put(udpListener.Players.Count);
                            dataWriter.Put(player.Position.X);
                            dataWriter.Put(player.Position.Y);

                            // TODO: Probably a better way of doing this...
                            /*for (int ii = 0; ii < udpListener.Players.Count; ii++)
                            {
                                dataWriter.Put(udpListener.Players[ii].Position.X);
                                dataWriter.Put(udpListener.Players[ii].Position.Y);
                            }*/
                            
                            var peer = player.Peer;
                            
                            if (peer != null)
                                peer.Send(dataWriter, DeliveryMethod.ReliableOrdered);
                        }
                    }
                }

                Thread.Sleep(TickRate);

                deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;
                time += deltaTime;

                previousTimer = currentTimer;
            }

            udpServer.Stop();
        }
    }
}
