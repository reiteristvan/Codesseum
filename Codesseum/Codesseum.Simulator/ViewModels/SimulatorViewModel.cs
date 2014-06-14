using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Codesseum.Common;
using Codesseum.Common.Types;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;

namespace Codesseum.Simulator.ViewModels
{
    public class SimulatorViewModel : ViewModelBase
    {
        public SimulatorViewModel(GameConfiguration configuration)
        {
            _cells = new ObservableCollection<CellViewModel>();
            _engine = new Engine(configuration);

            _engine.Events.CollectionChanged += GameEventHandler;

            StartCommand = new RelayCommand(Start);

            LoadMap(configuration.MapPath);
        }

        // Methods

        private void Start()
        {
            _engine.Start();
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

        private void GameEventHandler(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            foreach (GameEvent gameEvent in notifyCollectionChangedEventArgs.NewItems)
            {
                switch (gameEvent.Type)
                {
                    case EventType.BotSpawn:
                        HandleSpawnBot(gameEvent.BotId, gameEvent.BotAction.Target);
                        break;
                    case EventType.BotAction:
                        HandleBotAction(gameEvent.BotId, gameEvent.BotAction);
                        break;
                }
            }
        }

        private void HandleSpawnBot(Guid botId, Coordinate position)
        {
            var cell = Cells.FirstOrDefault(c => c.X == position.X && c.Y == position.Y);
            cell.IsOnBot = true;
            cell.BotId = botId;
        }

        private void HandleBotAction(Guid botId, BotAction action)
        {
            switch (action.Action)
            {
                case ActionType.Move:
                    MoveBot(botId, action.Target);
                    break;
                case ActionType.Attack:
                    break;
            }
        }

        private void MoveBot(Guid botId, Coordinate to)
        {
            var source = Cells.FirstOrDefault(c => c.BotId == botId);
            source.IsOnBot = false;
            source.BotId = Guid.Empty;

            var target = Cells.FirstOrDefault(c => c.X == to.X && c.Y == to.Y);
            target.IsOnBot = true;
            target.BotId = botId;
        }

        // Properties

        private ObservableCollection<CellViewModel> _cells;
        public ObservableCollection<CellViewModel> Cells
        {
            get { return _cells; }
            set { Set(() => Cells, ref _cells, value); }
        }  

        // Commands

        public RelayCommand StartCommand { get; private set; }

        // Fields

        private readonly Engine _engine;
    }
}
