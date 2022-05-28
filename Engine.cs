using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class Engine
    {
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public const int ScreenWidth = 1920;
        public const int ScreenHeight = 1080;

        public bool IsRunning;
        public bool IsPaused;

        public void Initialize()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Munita");
            Raylib.SetExitKey(KeyboardKey.KEY_Q);
            Raylib.SetTargetFPS(FPS);

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Initialize
            var player = new Player();
            player.Initialize();

            while (IsRunning)
            {
                if (Raylib.WindowShouldClose())
                    IsRunning = false;

                currentTimer = DateTime.Now;

                if (IsPaused)
                    deltaTime = 0.0f;
                else
                    deltaTime = FrameTimestep;
                //deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;

                time += deltaTime;

                // Update
                player.Update(deltaTime);

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                Raylib.BeginMode2D(player.Camera);

                player.Draw();

                Raylib.EndMode2D();

                Raylib.EndDrawing();

                previousTimer = currentTimer;
            }

            Raylib.CloseWindow();
        }
    }
}
