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
    public class Utils
    {
        public const int TicksPerSecond = 20;
        public const int TickRate = 1000 / TicksPerSecond;

        // Packet macros
        public static void SendPlayerUpdate(bool isRunning, Vector2 dir, string username)
        {
            Engine.Client.Send("PlayerUpdate", username, $"{isRunning}#{PackVec2(dir)}");
        }

        // Packet parsing
        public static string PackVec2(Vector2 vec)
        {
            return $"{vec.X},{vec.Y}";
        }
        
        public static Vector2 UnpackVec2(string text)
        {
            var vecBuffer = text.Split(",");

            var x = float.Parse(vecBuffer[0]);
            var y = float.Parse(vecBuffer[1]);

            return new Vector2(x, y);
        }

        public static byte[] GetBytes(string text)
        {
            return UTF8Encoding.ASCII.GetBytes(text);
        }

        public static string GetString(byte[] data)
        {
            return UTF8Encoding.ASCII.GetString(data);
        }
    }
}