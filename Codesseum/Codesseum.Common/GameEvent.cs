using System;
using Codesseum.Common.Entities;
using Codesseum.Common.Types;

namespace Codesseum.Common
{
    public class GameEvent
    {
        public EventType Type { get; set; }
        public BotAction BotAction { get; set; }
        public Guid BotId { get; set; }
        public Coordinate Position { get; set; }
        public ItemInformation ItemInformation { get; set; }
        public BotInformation BotInformation { get; set; }
    }

    public enum EventType
    {
        BotSpawn = 0,
        BotAction = 1,
        BotDead = 2,
        ItemSpawn = 10,
        ItemTaken = 11,
        StartOfTurn = 88,
        EndOfGame = 99
    }

    public class ItemInformation
    {
        public ItemType Type { get; internal set; }
        public PowerUpType PowerUpType { get; internal set; }
    }

    public class BotInformation
    {
        public int MaxHealth { get; internal set; }
        public int Health { get; internal set; }
        public string Team { get; internal set; }
    }
}
