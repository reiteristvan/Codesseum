using System.IO;

namespace Codesseum.Common
{
    public class Map
    {
        public Map(string mapName)
        {
            var lines = File.ReadAllLines(mapName);
            _map = new int[lines[0].Length, lines.Length];
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
                if (i < 0 || i > Width || j < 0 || j > Height)
                {
                    return -1;
                }

                return _map[i, j];
            }
        }

        private readonly int[,] _map;
    }
}
