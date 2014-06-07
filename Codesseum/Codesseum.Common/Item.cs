namespace Codesseum.Common
{
    public class Item
    {
        public Item(ItemType type, int value, PowerUpType powerUpType = PowerUpType.None)
        {
            Type = type;
            Value = value;
            PowerUpType = powerUpType;
        }

        public ItemType Type { get; private set; }
        public int Value { get; private set; }
        public PowerUpType PowerUpType { get; private set; }
    }

    public enum ItemType
    {
        Ammunition = 0,
        MediPack = 1,
        PowerUp = 2
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
