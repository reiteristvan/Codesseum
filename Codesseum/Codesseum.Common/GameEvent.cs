namespace Codesseum.Common
{
    public class GameEvent
    {
        public EventType Type { get; set; }
    }

    public enum EventType
    {
        BotAction = 0
    }
}
