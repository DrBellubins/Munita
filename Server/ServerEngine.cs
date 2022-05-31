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
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public bool IsRunning;
        public bool IsPaused;

        public void Initialize()
        {
            Debug.Log("Server engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Networking
            var udpListener = new ServerListener();
            var udpServer = new NetManager(udpListener);

            udpListener.Server = udpServer;

            udpServer.Start(25565);

            var dataWriter = new NetDataWriter();

            // Initialize
            var world = new World();
            world.Initialize(false);
            
            var player = new ServerPlayer();
            player.Initialize();

            while (IsRunning)
            {
                currentTimer = DateTime.Now;

                if (IsPaused)
                    deltaTime = 0.0f;
                else
                    deltaTime = FrameTimestep;
                    //deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;

                time += deltaTime;
                
                // Update
                udpServer.PollEvents();

                world.Update();
                player.Update(deltaTime);

                dataWriter.Reset();
                dataWriter.Put(player.Position.X);
                dataWriter.Put(player.Position.Y);

                if (udpServer.ConnectedPeersCount > 0)
                    udpServer.FirstPeer.Send(dataWriter, DeliveryMethod.ReliableOrdered);
                
                previousTimer = currentTimer;

                Thread.Sleep(15); // Doesn't update if too fast
            }

            udpServer.Stop();
        }
    }
}
