using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Codesseum.Common;
using Codesseum.Common.Entities;
using Codesseum.Common.Types;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;

namespace Codesseum.Simulator.ViewModels
{
    public class SimulatorViewModel : ViewModelBase
    {
        static public List<Brush> Colors = new List<Brush>
        {
            Brushes.Red, Brushes.Black, Brushes.Green, Brushes.MediumBlue
        }; 

        static public Dictionary<string, Brush> TeamColors = new Dictionary<string, Brush>();

        private readonly object _lock = new object();

        public SimulatorViewModel(GameConfiguration configuration)
        {
            TeamColors.Clear();

            // set logging

            FileStream logStream = null;

            if (File.Exists("log.txt"))
            {
                File.Delete("log.txt");
            }

            logStream = File.Create("log.txt");

            _log = new ObservableCollection<string>();
            _cells = new ObservableCollection<CellViewModel>();
            _engine = new Engine(configuration, logStream);

            SetColors();

            _engine.Events.CollectionChanged += GameEventHandler;

            StartCommand = new RelayCommand(Start, () => _isEngineRuns == false);

            LoadMap(configuration.MapPath);

            BindingOperations.EnableCollectionSynchronization(_engine.Points, _lock);
            BindingOperations.EnableCollectionSynchronization(Log, _lock);
        }

        // Methods
        private void Start()
        {
            _isEngineRuns = true;
            Task.Run(() => _engine.Start());
        }

        private void LoadMap(string path)
        {
            int x = 0;
            foreach (var line in File.ReadAllLines(path))
            {
                int y = 0;
                foreach (var c in line)
                {
                    Cells.Add(new CellViewModel(x, y, c == '1'));
                    ++y;
                }
                ++x;
            }
        }

        private void SetColors()
        {
            foreach (var team in _engine.Points)
            {
                TeamColors.Add(team.Key, Colors.First(c => !TeamColors.ContainsValue(c))); 
            }
        }

        private void GameEventHandler(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            foreach (GameEvent gameEvent in notifyCollectionChangedEventArgs.NewItems)
            {
                switch (gameEvent.Type)
                {
                    case EventType.BotSpawn:
                        HandleSpawnBot(gameEvent.BotId, gameEvent.BotAction.Target, gameEvent.BotInformation);
                        break;
                    case EventType.BotAction:
                        HandleBotAction(gameEvent.BotId, gameEvent.BotAction, gameEvent.BotInformation);
                        break;
                    case EventType.BotDead:
                        break;
                    case EventType.ItemSpawn:
                        HandleItemSpawn(gameEvent.Position, gameEvent.ItemInformation, true);
                        break;
                    case EventType.ItemTaken:
                        HandleItemSpawn(gameEvent.Position, gameEvent.ItemInformation, false);
                        break;
                    case EventType.EndOfGame:
                        HandleGameOver();
                        break;
                }
            }
        }

        private void HandleItemSpawn(Coordinate position, ItemInformation itemInformation, bool onPlace)
        {
            var cell = Cells.First(c => c.X == position.X && c.Y == position.Y);
            cell.IsOnItem = onPlace;
            cell.TeamColor = itemInformation == null ? null : GetItemColor(itemInformation);
        }

        private Brush GetItemColor(ItemInformation itemInformation)
        {
            // tresure

            switch (itemInformation.Type)
            {
                case ItemType.Special:
                    return Brushes.LightSkyBlue;
                case ItemType.Ammunition:
                    return Brushes.Gray;
                case ItemType.MediPack:
                    return Brushes.Magenta;
                case ItemType.PowerUp:
                    break;
            }

            switch (itemInformation.PowerUpType)
            {
                case PowerUpType.Health:
                    return Brushes.Red;
                case PowerUpType.Defense:
                    return Brushes.MediumBlue;
                case PowerUpType.Power:
                    return Brushes.Black;
                case PowerUpType.Speed:
                    return Brushes.OrangeRed;
            }

            return null;
        }

        private void HandleSpawnBot(Guid botId, Coordinate position, BotInformation botInformation)
        {
            var cell = Cells.FirstOrDefault(c => c.X == position.X && c.Y == position.Y);
            cell.IsOnBot = true;
            cell.BotId = botId;
            cell.Health = botInformation.Health;
            cell.TeamColor = TeamColors[botInformation.Team];
        }

        private void HandleBotAction(Guid botId, BotAction action, BotInformation botInformation)
        {
            switch (action.Action)
            {
                case ActionType.Move:
                    MoveBot(botId, action.Target, botInformation);
                    break;
                case ActionType.Attack:
                    AttackBot(botId, action.Target, botInformation);
                    break;
            }
        }

        private void MoveBot(Guid botId, Coordinate to, BotInformation botInformation)
        {
            Log.Add(string.Format("Move to {0}:{1}", to.X, to.Y));

            var source = Cells.FirstOrDefault(c => c.BotId == botId);
            source.IsOnBot = false;
            source.BotId = Guid.Empty;
            source.Health = 0;
            source.TeamColor = Constants.EmptyCellBrush;

            var target = Cells.FirstOrDefault(c => c.X == to.X && c.Y == to.Y);
            target.IsOnBot = true;
            target.BotId = botId;
            target.Health = botInformation.Health;
            target.TeamColor = TeamColors[botInformation.Team];
        }

        private void AttackBot(Guid botId, Coordinate target, BotInformation botInformation)
        {
            var sourceCell = Cells.FirstOrDefault(b => b.BotId == botId);

            // trigger and reverse animation
            sourceCell.IsAttacker = true;
            sourceCell.IsAttacker = false;

            var targetCell = Cells.FirstOrDefault(c => c.X == target.X && c.Y == target.Y);

            // trigger and reverse animation
            targetCell.IsAttacked = true;
            sourceCell.IsAttacked = false;
        }

        private void HandleGameOver()
        {
            _engine.Reset();
            _isEngineRuns = false;
        }

        // Properties
        public Engine Engine
        {
            get { return _engine; }
        }

        private ObservableCollection<CellViewModel> _cells;
        public ObservableCollection<CellViewModel> Cells
        {
            get { return _cells; }
            set { Set(() => Cells, ref _cells, value); }
        }

        private readonly ObservableCollection<string> _log; 
        public ObservableCollection<string> Log
        {
            get { return _log; }
        }

        // Commands

        public RelayCommand StartCommand { get; private set; }

        // Fields

        private readonly Engine _engine;
        private bool _isEngineRuns;
    }
}
