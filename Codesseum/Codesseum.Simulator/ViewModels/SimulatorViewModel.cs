using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Codesseum.Common;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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
