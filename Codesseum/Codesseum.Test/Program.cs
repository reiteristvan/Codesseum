using System;
using System.Collections.Generic;
using Codesseum.Common;

namespace Codesseum.Test
{
    class Program
    {
        static void Main()
        {
            var engine = new Common.Engine(new GameConfiguration
            {
                NumberOfTurns = 100,
                BotPathList = new List<string> { "TestBot.dll" },
                BotsPerTeam = 5,
                GameType = 1,
                MapPath = "testMap.txt"
            });

            engine.Start();

            Console.ReadKey();
        }
    }
}
