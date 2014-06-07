using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Codesseum.Common.Entities;
using Codesseum.Common.Types;
using Codesseum.Common.Map;

namespace Codesseum.Common
{
    public class Engine
    {
        public Engine(GameConfiguration configuration)
        {
            _configuration = configuration;
            _map = new GameMap(_configuration.MapPath);

            LoadBotTypes(_configuration.BotPathList);
        }

        public void Start()
        {
            if (_botTypes.Count < 1)
            {
                return;
            }

            // load map
            _map.Load();

            // initialize items
            SetItems();

            // initialize bots
            foreach (var botType in _botTypes)
            {
                for (var i = 0; i < _configuration.BotsPerTeam; ++i)
                {
                    var bot = (Bot)Activator.CreateInstance(botType);
                    bot.SetAttributes(bot.GetAttributes());

                    // set positions

                    _bots.Add(bot);
                    _points.Add(bot.TeamName, 0);
                }
            }

            int turn = 0;
            while (turn < _configuration.NumberOfTurns)
            {
                // set dead bots alive and reposition
                foreach (var deadBot in _bots.Where(b => b.IsDead))
                {
                    deadBot.IsDead = false;
                    deadBot.SetAttributes(deadBot.GetAttributes());

                    bool isCoordinateGood = false;
                    var random = new Random();

                    Coordinate newPosition = null;
                    while (isCoordinateGood == false)
                    {
                        var newCoordinate = new Coordinate(random.Next(_map.Width, _map.Height));

                        if (_map[newCoordinate] == 0)
                        {
                            if (!_bots.Any(b => b.Position.X == newCoordinate.X && b.Position.Y == newCoordinate.Y))
                            {
                                isCoordinateGood = true;
                                newPosition = newCoordinate;
                            }
                        }
                    }

                    deadBot.Position = newPosition;
                }

                // move
                foreach (var bot in _bots.OrderBy(b => b.Speed))
                {
                    if (bot.IsDead) { continue; }

                    var action = bot.NextAction(CreateWorldInfo());

                    // invalid move either case, no need to refresh world info
                    if (_map[action.Target] == -1)
                    {
                        continue;
                    }

                    var botOnCoordinate = _bots.FirstOrDefault(b => b.Position.Equals(action.Target));

                    if (action.Action == ActionType.Move)
                    {
                        // cell already taken
                        if (botOnCoordinate != null)
                        {
                            continue;
                        }

                        bot.Position = action.Target;
                    }
                    else // attack bot on target field
                    {
                        // invalid targets
                        if (botOnCoordinate == null)
                        {
                            continue;
                        }

                        // check if bot is in range

                        // attack

                        var damage = bot.Power - (botOnCoordinate.Defense/2);
                        botOnCoordinate.Health -= damage;

                        if (botOnCoordinate.Health <= 0)
                        {
                            _points[bot.TeamName] += 1;
                            botOnCoordinate.IsDead = true;
                        }
                    }
                }

                ++turn;
            }
        }

        private void SetItems()
        {
            var random = new Random();
            for (int i = 0; i < 5 - _items.Count; ++i)
            {
                bool l = false;
                while (!l)
                {
                    var c = Coordinate.CreateRandom(_map.Width, _map.Height, random);

                    if (_map[c] != -1)
                    {
                        _items.Add(
                            new Item(c, 
                                    (ItemType)random.Next(4), 
                                    random.Next(4), 
                                    (PowerUpType)random.Next(4)));
                        l = true;
                    }
                }
            }
        }

        private World CreateWorldInfo()
        {
            return new World
            {
                Bots = new List<Bot>(_bots),
                Items = new List<Item>(_items),
                Map = _map
            };
        }

        private void LoadBotTypes(IEnumerable<string> pathList)
        {
            foreach (var path in pathList)
            {
                var botType = LoadBotTypeFromAssembly(path);

                if (botType == null)
                {
                    throw new ArgumentException("Couldn't find type!");
                }

                _botTypes.Add(botType);
            }
        }
        private Type LoadBotTypeFromAssembly(string path)
        {
            var absolutePath = Directory.GetCurrentDirectory() + "\\" + path;
            var assembly = Assembly.LoadFile(absolutePath);
            var type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Contains(Path.GetFileNameWithoutExtension(absolutePath)));

            return type;
        }

        private readonly GameConfiguration _configuration;
        private readonly GameMap _map;
        private readonly List<Type> _botTypes = new List<Type>(); 
        private readonly List<Bot> _bots = new List<Bot>();
        private readonly List<Item> _items = new List<Item>(); 
        private readonly Dictionary<string, int> _points = new Dictionary<string, int>();
    }
}
