using System;

namespace Munita
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--server")
                {
                    // Server engine init
                    var serverEngine = new ServerEngine();
                    serverEngine.Initialize();
                }
                else
                {
                    var engine = new Engine();
                    engine.Initialize();
                }
            }
            else
            {
                var engine = new Engine();
                engine.Initialize();
                
            }
        }
    }
}