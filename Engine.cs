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
    public class Engine
    {
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public const int ScreenWidth = 1600;
        public const int ScreenHeight = 900;

        public static Font MainFont;

        public bool IsRunning;
        public bool IsPaused;

        public void Initialize()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Munita");
            Raylib.SetExitKey(KeyboardKey.KEY_Q);
            Raylib.SetTargetFPS(FPS);

            MainFont = Raylib.LoadFontEx("Assets/Font/VarelaRound-Regular.ttf", 64, null, 250);

            Debug.Log("Client engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Networking
            var udpListener = new ClientListener();

            var udpClient = new NetManager(udpListener)
            {
                SimulationMaxLatency = 1500,
                SimulateLatency = true
            };

            udpClient.Start();
            udpClient.Connect("localhost", 25565, "munita-client777");

            var dataWriter = new NetDataWriter();

            // Initialize
            var world = new World();
            world.Initialize(true);
            
            var player = new Player();
            player.Initialize();

            Debug.Initialize();

            while (IsRunning)
            {
                currentTimer = DateTime.Now;

                if (IsPaused)
                    deltaTime = 0.0f;
                else
                    deltaTime = FrameTimestep;
                    //deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;

                time += deltaTime;
                
                if (Raylib.WindowShouldClose())
                    IsRunning = false;
                
                // Update
                //udpListener.TestClientVec = player.MoveDirection;
                udpClient.PollEvents();

                dataWriter.Reset();
                dataWriter.Put(player.MoveDirection.X);
                dataWriter.Put(player.MoveDirection.Y);
                
                udpClient.FirstPeer.Send(dataWriter, DeliveryMethod.ReliableOrdered);

                world.Update();
                player.Update(true, deltaTime);

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                Raylib.BeginMode2D(player.Camera);

                world.Draw();
                player.Draw();

                Raylib.EndMode2D();

                Debug.Draw(time, deltaTime, player.Position);

                Raylib.EndDrawing();
                
                previousTimer = currentTimer;
            }

            udpClient.Stop();
            Raylib.CloseWindow();
        }
    }
}
