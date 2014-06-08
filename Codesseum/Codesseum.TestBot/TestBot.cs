using Codesseum.Common;
using Codesseum.Common.Entities;
using Codesseum.Common.Types;

namespace Codesseum.TestBot
{
    public class TestBot : Bot
    {
        public TestBot()
        {
            TeamName = "almabeka";
        }

        public override BotAction NextAction(World world)
        {
            return new BotAction
            {
                Action = ActionType.Move,
                Target = Coordinate.CreateRandom(world.Map.Width, world.Map.Height)
            };
        }

        public override int[] GetAttributes()
        {
            return new[] { 5, 5, 5, 5, 5 };
        }
    }
}
