using System;
using System.Collections.Generic;
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
            _engine = new Engine(configuration);

            StartCommand = new RelayCommand(Start);
        }

        // Methods

        private void Start()
        {
            _engine.Start();
        }

        // Commands

        public RelayCommand StartCommand { get; private set; }

        // Fields

        private readonly Engine _engine;
    }
}
