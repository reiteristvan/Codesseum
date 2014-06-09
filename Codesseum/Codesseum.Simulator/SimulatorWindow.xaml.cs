using System.Collections.Generic;
using Codesseum.Common;
using Codesseum.Simulator.Messages;
using Codesseum.Simulator.Models;
using Codesseum.Simulator.ViewModels;
using GalaSoft.MvvmLight.Messaging;

namespace Codesseum.Simulator
{
    /// <summary>
    /// Interaction logic for SimulatorWindow.xaml
    /// </summary>
    public partial class SimulatorWindow
    {
        public SimulatorWindow()
        {
            InitializeComponent();
        }

        public void Show(GameConfiguration configuration)
        {
            DataContext = new SimulatorViewModel(configuration);
            Show();
        }
    }
}
