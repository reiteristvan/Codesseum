using System;
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
            var random = new Random();
            Coordinate target;

            if (random.Next(1000)%2 == 0)
            {
                target = new Coordinate(Position.X + 5, Position.Y);
            }
            else
            {
                target = new Coordinate(Position.X, Position.Y + 5);
            }

            return new BotAction
            {
                Action = ActionType.Move,
                Target = target
            };
        }

        public override int[] GetAttributes()
        {
            return new[] { 5, 5, 5, 5, 5 };
        }
    }
}
