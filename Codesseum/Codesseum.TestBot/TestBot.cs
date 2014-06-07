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

        public override BotAction NextAction()
        {
            return null;
        }

        public override int[] GetAttributes()
        {
            throw new System.NotImplementedException();
        }
    }
}
