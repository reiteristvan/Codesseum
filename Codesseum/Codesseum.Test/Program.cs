using System;
using System.Collections.Generic;
using System.IO;
using Codesseum.Common;

namespace Codesseum.Test
{
    class Program
    {
        static void Main()
        {
            var engine = new Engine(new GameConfiguration
            {
                NumberOfTurns = 50,
                BotPathList = new List<string> { Directory.GetCurrentDirectory() + "\\TestBot.dll" },
                BotsPerTeam = 5,
                GameType = 1,
                MapPath = "testMap.txt",
                Speed = 10
            }, Console.OpenStandardOutput());

            engine.Start();

            Console.ReadKey();
        }
    }
}
