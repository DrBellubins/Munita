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

            // Initialize
            var world = new World();
            world.Initialize(false);
            
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
                
                // Update
                world.Update();
                player.Update(false, deltaTime);
                
                previousTimer = currentTimer;
            }
        }
    }
}
