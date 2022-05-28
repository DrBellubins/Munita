using System;

namespace Munita
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var engine = new Engine();

            if (args.Length > 0)
            {
                if (args[0] == "--server")
                    engine.Initialize(false);
            }
            else
                engine.Initialize(true);

            Console.WriteLine("Engine started!");
        }
    }
}