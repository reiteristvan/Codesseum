using System.IO;
using Codesseum.Common.Types;

namespace Codesseum.Common.Map
{
    public class GameMap
    {
        public GameMap(string mapPath)
        {
            _path = mapPath;
        }

        public void Load()
        {
            var lines = File.ReadAllLines(_path);
            _map = new int[lines.Length, lines[0].Length];
            Width = lines[0].Length;
            Height = lines.Length;

            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    _map[i, j] = int.Parse(lines[i][j].ToString());
                }
            }
        }

        public int Width { get; internal set; }
        public int Height { get; internal set; }

        public int this[int x, int y]
        {
            get
            {
                // invalid coordinate
                if (x < 0 || x >= Height || y < 0 || y >= Width)
                {
                    return -1;
                }

                // wall
                if (_map[x, y] == 1)
                {
                    return -1;
                }

                return _map[x, y];
            }
        }

        public int this[Coordinate coordinate]
        {
            get
            {
                return this[coordinate.X, coordinate.Y];
            }
        }

        private readonly string _path;
        private int[,] _map;
    }
}
