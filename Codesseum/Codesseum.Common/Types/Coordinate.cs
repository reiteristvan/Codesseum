using System;

namespace Codesseum.Common.Types
{
    public class Coordinate
    {
        static public Coordinate CreateRandom(int maxWidth, int maxHeight, Random random = null)
        {
            var _r = random ?? new Random();
            return new Coordinate(_r.Next(0, maxWidth), _r.Next(0, maxHeight));
        }

        public Coordinate(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }

        public override bool Equals(object obj)
        {
            var _c = (Coordinate) obj;

            return _c.X == X && _c.Y == Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", X, Y);
        }
    }
}
