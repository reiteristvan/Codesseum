using System;
using Codesseum.Common;

namespace Codesseum.Test
{
    class Program
    {
        static void Main()
        {
            //var map = new Map("testMap.txt");

            var engine = new Common.Engine(new GameConfiguration());
            //engine.LoadBots(new List<string> { "TestBot.dll" });

            Console.ReadKey();
        }
    }
}
