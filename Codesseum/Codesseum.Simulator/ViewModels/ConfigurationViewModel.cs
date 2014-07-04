using System.IO;
using GalaSoft.MvvmLight;

namespace Codesseum.Simulator.ViewModels
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private string _mapPath = "";
        public string MapPath
        {
            get { return _mapPath; }
            set { Set(() => MapPath, ref _mapPath, value); }
        }

        private string _numberOfTurns = "100";
        public string NumberOfTurns
        {
            get { return _numberOfTurns; }
            set { Set(() => NumberOfTurns, ref _numberOfTurns, value); }
        }

        private string _botsPerTeam = "5";
        public string BotsPerTeam
        {
            get { return _botsPerTeam; }
            set { Set(() => BotsPerTeam, ref _botsPerTeam, value); }
        }

        private string _gameType = "0";
        public string GameType
        {
            get { return _gameType; }
            set { Set(() => GameType, ref _gameType, value); }
        }

        private string _interval = "100";
        public string Interval
        {
            get { return _interval; }
            set { Set(() => Interval, ref _interval, value); }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(MapPath) || !File.Exists(MapPath))
            {
                return false;
            }

            return !string.IsNullOrEmpty(NumberOfTurns) && !string.IsNullOrEmpty(BotsPerTeam) && 
                   !string.IsNullOrEmpty(GameType) && !string.IsNullOrEmpty(Interval);
        }
    }
}
