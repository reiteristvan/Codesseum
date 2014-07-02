using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
            Events = new ObservableCollection<GameEvent>();

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
                    bot.Id = Guid.NewGuid();
                    bot.IsDead = false;

                    _bots.Add(bot);

                    Events.Add(new GameEvent
                    {
                        BotId = bot.Id,
                        Type = EventType.BotSpawn,
                        BotAction = new BotAction { Target  = bot.Position }
                    });
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

                    _logger.Log(string.Format("{0}@{1} respawned at {2}", 
                        deadBot.TeamName, deadBot.Id, deadBot.Position));
                }

                // initialize items
                SetItems();

                // move
                foreach (var bot in _bots.OrderBy(b => b.Speed))
                {
                    if (bot.IsDead) { continue; }

                    var action = bot.NextAction(CreateWorldInfo());

                    // invalid move either case, no need to refresh world info as nothing changed
                    if (_map[action.Target] == -1 || 
                        !IsTwoCoordinateInLine(bot.Position, action.Target) ||
                        IsThereBlockBetweenCoordinates(bot.Position, action.Target))
                    {
                        continue;
                    }

                    var source = new Coordinate(bot.Position.X, bot.Position.Y);
                    var botOnCoordinate = _bots.FirstOrDefault(b => b.Position.Equals(action.Target));

                    if (action.Action == ActionType.Move)
                    {
                        // cell already taken, invalid move
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
                                    _logger.Log(string.Format("{0}@{1} taken ammunition", bot.TeamName, bot.Id));
                                    break;
                                case ItemType.MediPack:
                                    bot.Health += item.Value;
                                    _logger.Log(string.Format("{0}@{1} taken a medipack", bot.TeamName, bot.Id));
                                    break;
                                case ItemType.PowerUp:
                                    break;
                                case ItemType.Special: // treasure hunt mode
                                    var team = _points.First(b => b.Key == bot.TeamName);
                                    team.Value += item.Value;
                                    _logger.Log(string.Format("{0}@{1} taken a treasure and gain {2} points", 
                                        bot.TeamName, bot.Id, item.Value));
                                    break;
                            }

                            _items.Remove(item);

                            Events.Add(new GameEvent
                            {
                                Type = EventType.ItemTaken,
                                Position = item.Position
                            });
                        }

                        _logger.Log(string.Format("{0}@{1} moved from {2} to {3}", 
                            bot.TeamName, bot.Id, source, action.Target));

                        Events.Add(new GameEvent
                        {
                            BotAction = action,
                            BotId = bot.Id,
                            Type = EventType.BotAction
                        });
                        
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
                            Math.Abs(action.Target.Y - bot.Position.Y) > bot.Range)
                        {
                            continue;
                        }

                        // attack

                        var damage = bot.Power - (botOnCoordinate.Defense / 2);
                        --bot.Ammunition;
                        botOnCoordinate.Health -= damage;

                        _logger.Log(string.Format("{0}@{1} attacked {2}@{3} with damage: {4}", 
                            bot.TeamName, bot.Id, botOnCoordinate.TeamName, botOnCoordinate.Id, damage));

                        if (botOnCoordinate.Health <= 0)
                        {
                            var team = _points.First(b => b.Key == bot.TeamName);
                            team.Value += 1;
                            botOnCoordinate.IsDead = true;

                            _logger.Log(string.Format("{0}@{1} has died", 
                                botOnCoordinate.TeamName, botOnCoordinate.Id));

                            Events.Add(new GameEvent
                            {
                                Type = EventType.BotDead,
                                BotId = botOnCoordinate.Id
                            });
                        }
                    }
                }

                ++turn;
                Thread.Sleep(_configuration.Speed);
            }

            Events.Add(new GameEvent
            {
                Type = EventType.EndOfGame
            });
        }

        public void Reset()
        {
            _bots.Clear();
            _items.Clear();

            foreach (var team in _points)
            {
                team.Value = 0;
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
            
            while (i != rhs.Y)
            {
                if (_map[lhs.X, i] != 0)
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

            while (i != rhs.X)
            {
                if (_map[i, lhs.Y] != 0)
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
                    var botOnCoordinate = _bots.FirstOrDefault(b => b.Position.Equals(c));

                    if (_map[c] != -1 && botOnCoordinate == null)
                    {
                        _items.Add(
                            new Item(c,
                                (ItemType) random.Next(4),
                                random.Next(4),
                                (PowerUpType) random.Next(4)));
                        l = true;

                        Events.Add(new GameEvent
                        {
                            Type = EventType.ItemSpawn,
                            Position = c
                        });
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

                var bot = (Bot) Activator.CreateInstance(botType);

                if (_points.All(b => b.Key != bot.TeamName))
                {
                    _points.Add(new Pair(bot.TeamName, 0));
                }

                _botTypes.Add(botType);
            }
        }
        private Type LoadBotTypeFromAssembly(string path)
        {
            var assembly = Assembly.LoadFile(path);
            var type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Contains(Path.GetFileNameWithoutExtension(path)));

            return type;
        }

        // Properties

        public ObservableCollection<GameEvent> Events { get; private set; } 

        public GameMap Map
        {
            get { return _map; }
        }

        public List<Bot> Bots
        {
            get { return _bots; }
        }

        public List<Item> Items
        {
            get { return _items; }
        }

        public ObservableCollection<Pair> Points
        {
            get { return _points; }
        } 

        private readonly GameConfiguration _configuration;
        private readonly GameMap _map;
        private readonly List<Type> _botTypes = new List<Type>(); 
        private readonly List<Bot> _bots = new List<Bot>();
        private readonly List<Item> _items = new List<Item>(); 
        private readonly ObservableCollection<Pair> _points = new ObservableCollection<Pair>();
        private readonly Logger _logger;
    }
}
