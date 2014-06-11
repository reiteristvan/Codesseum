using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codesseum.Common;
using Codesseum.Simulator.Messages;
using Codesseum.Simulator.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Codesseum.Simulator.ViewModels
{
    public class SimulatorViewModel : ViewModelBase
    {
        public SimulatorViewModel(GameConfiguration configuration)
        {
            _cells = new ObservableCollection<CellViewModel>();
            _engine = new Engine(configuration);

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
