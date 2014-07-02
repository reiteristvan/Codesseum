using System.Collections.Generic;
using Codesseum.Common.Entities;
using Codesseum.Common.Map;

namespace Codesseum.Common
{
    public class World
    {
        static public World Current { get; internal set; }

        public List<Item> Items { get; internal set; }
        public List<Bot> Bots { get; internal set; }
        public GameMap Map { get; internal set; }
    }
}
