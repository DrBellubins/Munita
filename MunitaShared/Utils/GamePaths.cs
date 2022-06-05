using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Munita
{
    public class GamePaths
    {
        public static string Assets
        {
            get { return $"{Environment.CurrentDirectory}\\Assets"; }
        }

        public static string Textures
        {
            get { return $"{Environment.CurrentDirectory}\\Assets\\Textures"; }
        }

        public static string Maps
        {
            get { return $"{Environment.CurrentDirectory}\\Assets\\Maps"; }
        }

        public static string Tilesets
        {
            get { return $"{Environment.CurrentDirectory}\\Assets\\Tilesets"; }
        }

        public static string ServerWorldPath
        {
            get { return $"{Environment.CurrentDirectory}\\World"; }
        }
    }
}