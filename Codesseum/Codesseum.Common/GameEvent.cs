using System;
using Codesseum.Common.Types;

namespace Codesseum.Common
{
    public class GameEvent
    {
        public EventType Type { get; set; }
        public BotAction BotAction { get; set; }
        public Guid BotId { get; set; }
        public Coordinate Position { get; set; }
    }

    public enum EventType
    {
        BotSpawn = 0,
        BotAction = 1,
        BotDead = 2,
        ItemSpawn = 10,
        ItemTaken = 11,
        EndOfGame = 99
    }
}
