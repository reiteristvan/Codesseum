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

            for (int i = 0; i < lines.Length; ++i)
            {
                for (int j = 0; j < lines[0].Length; ++j)
                {
                    _map[i, j] = int.Parse(lines[i][j].ToString());
                }
            }
        }

        public int Width { get; internal set; }
        public int Height { get; internal set; }

        public int this[int i, int j]
        {
            get
            {
                // invalid coordinate
                if (i < 0 || i > Height || j < 0 || j > Width)
                {
                    return -1;
                }

                return _map[i, j];
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
