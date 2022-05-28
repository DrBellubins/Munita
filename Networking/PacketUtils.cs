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
    public class PacketUtils
    {
        public static byte[]? PackPlayerPosition(Vector2 position)
        {
            var xBytes = BitConverter.GetBytes(position.X);
            var yBytes = BitConverter.GetBytes(position.Y);

            return xBytes.Concat(yBytes).ToArray();
        }

        public static Vector2 UnpackPlayerPosition(byte[] positionBytes)
        {
            return new Vector2(BitConverter.ToSingle(positionBytes, 0),
                BitConverter.ToSingle(positionBytes, 4));
        }
    }
}