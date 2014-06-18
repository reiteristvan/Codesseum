using System.Collections.Generic;

namespace Codesseum.Common
{
    public class GameConfiguration
    {
        public List<string> BotPathList { get; set; }
        public string MapPath { get; set; }
        public int NumberOfTurns { get; set; }
        public int BotsPerTeam { get; set; }
        public int GameType { get; set; }
        public int Speed { get; set; }
    }
}
