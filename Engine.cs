using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;
using System.Net.Sockets;

using Raylib_cs;

namespace Munita
{
    // TODO: UDP stuff blocks updates until data is received... client/server don't update.
    public class Engine
    {
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;

        public bool IsRunning;
        public bool IsPaused;

        public static Vector2 MoveDirFromClient;
        public static Vector2 PosFromServer;

        public void Initialize(bool isClient)
        {
            var client = new UdpClient();
            var server = new UdpClient();

            var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25565);
            var remoteEndpoint = new IPEndPoint(IPAddress.Any, 25565);

            if (isClient)
            {
                client = new UdpClient();
                client.Connect(endPoint);

                Raylib.InitWindow(ScreenWidth, ScreenHeight, "Munita");
                Raylib.SetExitKey(KeyboardKey.KEY_Q);
                Raylib.SetTargetFPS(FPS);
            }
            else
            {
                server = new UdpClient(25565);
            }

            var dataFromServer = new byte[] {};
            var dataFromClient = new byte[] {};

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Initialize
            var world = new World();
            world.Initialize(isClient);
            
            var player = new Player();
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
                
                if (isClient)
                {
                    dataFromServer = new byte[] {};
                    dataFromServer = client.Receive(ref endPoint);

                    if (Raylib.WindowShouldClose())
                        IsRunning = false;

                    // Update
                    PosFromServer = PacketUtils.UnpackPlayerPosition(dataFromServer);

                    world.Update();
                    player.Update(isClient, deltaTime);

                    var moveBytes = PacketUtils.PackPlayerPosition(player.Position);
                    
                    if (moveBytes != null)
                        server.Send(moveBytes, moveBytes.Length, remoteEndpoint);

                    // Draw
                    Raylib.BeginDrawing();
                    Raylib.ClearBackground(Color.BLACK);

                    Raylib.BeginMode2D(player.Camera);

                    world.Draw();
                    player.Draw();

                    Raylib.EndMode2D();

                    Raylib.EndDrawing();
                }
                else // Server
                {
                    dataFromClient = new byte[] {};
                    dataFromClient = server.Receive(ref remoteEndpoint);

                    // Update
                    MoveDirFromClient = PacketUtils.UnpackPlayerPosition(dataFromClient);

                    world.Update();
                    player.Update(isClient, deltaTime);

                    var posBytes = PacketUtils.PackPlayerPosition(player.Position);

                    if (posBytes != null)
                        server.Send(posBytes, posBytes.Length, remoteEndpoint);
                }
                
                previousTimer = currentTimer;
            }

            if (isClient)
                Raylib.CloseWindow();
        }
    }
}
