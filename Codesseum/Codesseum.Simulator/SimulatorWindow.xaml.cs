using Codesseum.Common;
using Codesseum.Simulator.ViewModels;

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
