using System;

namespace Munita
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--client")
                {
                    var clientEngine = new ClientEngine();
                    clientEngine.Initialize();
                }
                else
                {
                    // Server engine init
                    var serverEngine = new ServerEngine();
                    serverEngine.Initialize();
                }
            }
            else
            {
                // Server engine init
                var serverEngine = new ServerEngine();
                serverEngine.Initialize();
            }
            /*if (args.Length > 0)
            {
                if (args[0] == "--server")
                {
                    // Server engine init
                    var serverEngine = new ServerEngine();
                    serverEngine.Initialize();
                }
                else
                {
                    var clientEngine = new ClientEngine();
                    clientEngine.Initialize();
                }
            }
            else
            {
                var clientEngine = new ClientEngine();
                clientEngine.Initialize();
            }*/
        }
    }
}