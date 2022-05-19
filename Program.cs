using System;

namespace Munita
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var engine = new Engine();
            engine.Initialize();

            Console.WriteLine("Engine started!");
        }
    }
}