using Codesseum.Common.Types;

namespace Codesseum.Common.Entities
{
    public class Item
    {
        public Item(Coordinate position, ItemType type, int value, PowerUpType powerUpType = PowerUpType.None)
        {
            Position = position;
            Type = type;
            Value = value;
            PowerUpType = powerUpType;
        }

        public Coordinate Position { get; private set; }
        public ItemType Type { get; private set; }
        public int Value { get; private set; }
        public PowerUpType PowerUpType { get; private set; }
    }

    public enum ItemType
    {
        Ammunition = 0,
        MediPack = 1,
        PowerUp = 2,
        Special = 3
    }

    public enum PowerUpType
    {
        None = 99,
        Health = 0,
        Power = 1,
        Defense = 2,
        Speed = 3
    }
}
