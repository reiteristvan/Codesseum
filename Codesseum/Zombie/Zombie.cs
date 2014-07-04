using System;
using System.Linq;
using Codesseum.Common;
using Codesseum.Common.Entities;
using Codesseum.Common.Types;

namespace Zombie
{
    public class Zombie : Bot
    {
        public Zombie()
        {
            TeamName = "Zombie";
        }

        private Coordinate GetDirection(Direction direction, Coordinate origin)
        {
            Coordinate result;

            switch (direction)
            {
                case Direction.Down:
                    result = new Coordinate(origin.X, origin.Y + 1);
                    break;
                case Direction.Left:
                    result = new Coordinate(origin.X - 1, origin.Y);
                    break;
                case Direction.Right:
                    result = new Coordinate(origin.X + 1, origin.Y);
                    break;
                case Direction.Top:
                    result = new Coordinate(origin.X, origin.Y - 1);
                    break;
                default:
                    return origin;
            }

            return result;
        }

        public override BotAction NextAction()
        {
            var result = new BotAction { Action = ActionType.Move };

            var botToAttack = GetBotInRange();
            if (botToAttack != null)
            {
                result.Action = ActionType.Attack;
                result.Target = botToAttack.Position;
                return result;
            }

            bool success = false;

            while (!success)
            {
                Direction direction;
                lock (_locker)
                {
                    direction = (Direction)random.Next(0, 4);
                }

                var target = GetDirection(direction, Position);

                if (World.Current.Map[target] != -1)
                {
                    result.Target = target;
                    success = true;
                }
            }

            return result;
        }

        public override int[] GetAttributes()
        {
            return new[] { 9, 6, 2, 7, 1 };
        }

        private Bot GetBotInRange()
        {
            foreach (var bot in World.Current.Bots.Where(b => b.TeamName != TeamName))
            {
                if (IsBotInRange(Position, bot.Position))
                {
                    return bot;
                }
            }

            return null;
        }

        private bool IsBotInRange(Coordinate source, Coordinate target)
        {
            if (Math.Abs(target.X - source.X) > Range ||
                Math.Abs(target.Y - source.Y) > Range)
            {
                return false;
            }

            return true;
        }

        private static readonly  object _locker = new object();
        private static readonly Random random = new Random();
    }
    public enum Direction
    {
        Left = 0,
        Right = 1,
        Top = 2,
        Down = 3
    }
}
