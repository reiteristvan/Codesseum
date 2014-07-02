using System;

namespace Codesseum.Common
{
    public class GameEvent
    {
        public EventType Type { get; set; }
        public BotAction BotAction { get; set; }
        public Guid BotId { get; set; }
    }

    public enum EventType
    {
        BotSpawn = 0,
        BotAction = 1,
        ItemSpawn = 2,
        EndOfGame = 99
    }
}
