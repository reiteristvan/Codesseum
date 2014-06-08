using System;
using System.Collections.Generic;
using Codesseum.Common;

namespace Codesseum.Test
{
    class Program
    {
        static void Main()
        {
            var engine = new Engine(new GameConfiguration
            {
                NumberOfTurns = 1000,
                BotPathList = new List<string> { "TestBot.dll" },
                BotsPerTeam = 5,
                GameType = 1,
                MapPath = "testMap.txt"
            }, Console.OpenStandardOutput());

            engine.Start();

            Console.ReadKey();
        }
    }
}
