using Codesseum.Common;
using Codesseum.Common.Entities;

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
            return null;
        }

        public override int[] GetAttributes()
        {
            return new[] { 5, 5, 5, 5 };
        }
    }
}
