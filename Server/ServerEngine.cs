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

            udpServer.Start(25565);

            // Initialize
            var world = new World();
            world.Initialize(false);

            while (IsRunning)
            {
                currentTimer = DateTime.Now;

                deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;
                time += deltaTime;
                
                // Update
                udpServer.PollEvents();

                world.Update();

                if (udpServer.ConnectedPeersCount > 0)
                {
                    for (int i = 0; i < udpListener.Players.Count; i++)
                    {
                        var dataWriter = new NetDataWriter();

                        udpListener.Players[i].Update(deltaTime);

                        dataWriter.Put(udpListener.Players[i].Position.X);
                        dataWriter.Put(udpListener.Players[i].Position.Y);

                        udpServer.FirstPeer.Send(dataWriter, DeliveryMethod.ReliableOrdered);
                    }
                }
                
                Thread.Sleep(TickRate); // Update 20 times a second
                previousTimer = currentTimer;
            }

            udpServer.Stop();
        }
    }
}
