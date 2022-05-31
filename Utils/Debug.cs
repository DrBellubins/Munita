using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class Debug
    {
        private static float debugUpdateTimer = 0.0f;
        private static float debugUpdatePeriod = 0.25f;
        private static float fpsDisplay = 0.0f;

        private static List<string> debugTextQueue = new List<string>();

        public static void Initialize()
        {
            
        }

        public static void Draw(float time, float deltaTime, Vector2 playerPosition)
        {
            // Debug text
            if (time > debugUpdateTimer)
            {
                debugUpdateTimer = time + debugUpdatePeriod;
                fpsDisplay = 1.0f / deltaTime;
            }

            UI.DrawText($"FPS: {fpsDisplay}", 0f, 0f);
            UI.DrawText($"Position: {playerPosition.X}, {playerPosition.Y}", 0f, 24f);

            for (int i = 0; i < debugTextQueue.Count; i++)
            {
                UI.DrawText(debugTextQueue[i], 0f, 70 + (i * 28));
            }

            debugTextQueue.Clear();
        }

        public static void DrawText(string text)
        {
            debugTextQueue.Add(text);
        }

        public static void WriteVector2(string prefix, Vector2 input)
        {
            Console.WriteLine($"{prefix}: {input.X}, {input.Y}");
        }

        public static void Draw2DGrid(Vector2 position, int slices, float spacing)
        {
            Rlgl.rlPushMatrix();
            Rlgl.rlTranslatef(position.X, position.Y, 0f);
            Rlgl.rlRotatef(90f, 1f, 0f, 0f);

            Raylib.DrawGrid(slices, spacing);

            Rlgl.rlPopMatrix();
        }

        public static void Log(string log)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(log);
        }

        public static void Announce(string announcement)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(announcement);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Warning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(warning);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
