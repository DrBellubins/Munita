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
    public class Engine
    {
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public const int ScreenWidth = 1600;
        public const int ScreenHeight = 900;

        public static Vector2 MoveDirFromClient;
        public static Vector2 PosFromServer;

        public static Font MainFont;

        public bool IsRunning;
        public bool IsPaused;

        public void Initialize()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Munita");
            Raylib.SetExitKey(KeyboardKey.KEY_Q);
            Raylib.SetTargetFPS(FPS);

            MainFont = Raylib.LoadFontEx("Assets/Font/VarelaRound-Regular.ttf", 64, null, 250);

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

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

            Raylib.CloseWindow();
        }
    }
}
