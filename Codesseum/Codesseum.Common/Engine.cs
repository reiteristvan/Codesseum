using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Codesseum.Common.Entities;
using Codesseum.Common.Log;
using Codesseum.Common.Types;
using Codesseum.Common.Map;

namespace Codesseum.Common
{
    public class Engine
    {
        public Engine(GameConfiguration configuration, Stream logOutput = null)
        {
            _logger = new Logger(logOutput);
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

            // initialize bots
            foreach (var botType in _botTypes)
            {
                for (var i = 0; i < _configuration.BotsPerTeam; ++i)
                {
                    var bot = (Bot)Activator.CreateInstance(botType);

                    bot.SetAttributes(bot.GetAttributes());
                    bot.Position = GetRandomBotPosition();

                    _bots.Add(bot);

                    if (!_points.ContainsKey(bot.TeamName))
                    {
                        _points.Add(bot.TeamName, 0);
                    }
                }
            }

            int turn = 0;
            while (turn < _configuration.NumberOfTurns)
            {
                _logger.Log(string.Format("Turn: {0}", turn));

                // set dead bots alive and reposition
                foreach (var deadBot in _bots.Where(b => b.IsDead))
                {
                    deadBot.IsDead = false;
                    deadBot.SetAttributes(deadBot.GetAttributes());
                    deadBot.Position = GetRandomBotPosition();
                }

                // initialize items
                SetItems();

                // move
                foreach (var bot in _bots.OrderBy(b => b.Speed))
                {
                    if (bot.IsDead) { continue; }

                    var action = bot.NextAction(CreateWorldInfo());

                    // invalid move either case, no need to refresh world info
                    if (_map[action.Target] == -1 || 
                        !IsTwoCoordinateInLine(bot.Position, action.Target) ||
                        IsThereBlockBetweenCoordinates(bot.Position, action.Target))
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

                        // check for items and powerups
                        if (IsItemOnCoordinate(action.Target))
                        {
                            var item = _items.First(i => i.Position.Equals(action.Target));

                            switch (item.Type)
                            {
                                case ItemType.Ammunition:
                                    bot.Ammunition += item.Value;
                                    break;
                                case ItemType.MediPack:
                                    bot.Health += item.Value;
                                    break;
                                case ItemType.PowerUp:
                                    break;
                            }
                        }
                    }
                    else // attack bot on target field
                    {
                        // invalid targets or no ammunition or teammate
                        if (botOnCoordinate == null || 
                            bot.Ammunition == 0 || 
                            botOnCoordinate.TeamName == bot.TeamName)
                        {
                            continue;
                        }

                        // check if bot is in range
                        if (Math.Abs(action.Target.X - bot.Position.X) > bot.Range ||
                            Math.Abs(action.Target.Y - bot.Position.Y) > bot.Range
                            )
                        {
                            continue;
                        }

                        // attack

                        var damage = bot.Power - (botOnCoordinate.Defense/2);
                        --bot.Ammunition;
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

        private Coordinate GetRandomBotPosition()
        {
            Coordinate result = null;
            bool l = false;
            var random = new Random();

            while (!l)
            {
                var _c = Coordinate.CreateRandom(_map.Width, _map.Height, random);

                if (_map[_c] != -1 && !IsBotOnCoordinate(_c) && !IsItemOnCoordinate(_c))
                {
                    result = _c;
                    l = true;
                }
            }

            return result;
        }

        private bool IsItemOnCoordinate(Coordinate coordinate)
        {
            return _items.Any(i => i.Position.Equals(coordinate));
        }

        private bool IsBotOnCoordinate(Coordinate coordinate)
        {
            return _bots.Any(b => b.Position.Equals(coordinate));
        }

        private bool IsTwoCoordinateInLine(Coordinate lhs, Coordinate rhs)
        {
            return lhs.X == rhs.X || lhs.Y == rhs.Y;
        }

        private bool IsThereBlockBetweenCoordinates(Coordinate lhs, Coordinate rhs)
        {
            return lhs.X == rhs.X ? CheckForBlockVertical(lhs, rhs) : CheckForBlockHorizontal(lhs, rhs);
        }

        // Y coordinates
        private bool CheckForBlockVertical(Coordinate lhs, Coordinate rhs)
        {
            int step = lhs.Y > rhs.Y ? -1 : 1;
            int i = lhs.Y + step;
            
            while (i == rhs.Y)
            {
                if (_map[lhs.X, i] == -1)
                {
                    return true;
                }

                i += step;
            }

            return false;
        }

        // X coordinates
        private bool CheckForBlockHorizontal(Coordinate lhs, Coordinate rhs)
        {
            int step = lhs.X > rhs.X ? -1 : 1;
            int i = lhs.X + step;

            while (i == rhs.X)
            {
                if (_map[i, lhs.Y] == -1)
                {
                    return true;
                }

                i += step;
            }

            return false;
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
        private readonly Logger _logger;
    }
}
