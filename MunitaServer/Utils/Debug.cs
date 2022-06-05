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
