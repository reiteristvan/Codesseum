namespace Codesseum.Common
{
    public class Coordinate
    {
        public Coordinate(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
    }
}
