using Codesseum.Common.Types;

namespace Codesseum.Common
{
    public class BotAction
    {
        public Coordinate Target { get; set; }
        public ActionType Action { get; set; }
    }

    public enum ActionType
    {
        Move = 0,
        Attack = 1
    }
}
