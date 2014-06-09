using System.Collections.Generic;
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

        public void Show(IEnumerable<BotInfoModel> bots)
        {
            DataContext = new SimulatorViewModel(bots);
            Show();

            Messenger.Default.Send(new StartMessage());
        }
    }
}
