using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Codesseum.Common;
using Codesseum.Common.Entities;
using Codesseum.Simulator.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace Codesseum.Simulator.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            AddMapCommand = new RelayCommand(AddMap);
            AddBotCommand = new RelayCommand(AddBot);
            RemoveBotCommand = new RelayCommand(RemoveBot);
            StartSimulationCommand = new RelayCommand(StartSimulation);

            _botInformations = new ObservableCollection<BotInfoModel>();
            _logs = new ObservableCollection<string>();
            Configuration = new ConfigurationViewModel();
        }

        private void AddMap()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt) | *.txt";

            if (dialog.ShowDialog() == true)
            {
                Configuration.MapPath = dialog.FileName;
            }
        }

        private void AddBot()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "C# assemblies (*.dll)|*.dll";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    if (!IsAssemblyValid(fileName))
                    {
                        Logs.Add("Invalid assembly");
                        continue;
                    }

                    Logs.Add("Bot loaded");

                    BotInformations.Add(new BotInfoModel
                    {
                        Path = fileName,
                        TeamName = ExtractTeamName(fileName)
                    });
                }
            }
        }

        private void RemoveBot()
        {
            var botToRemove = _botInformations.FirstOrDefault(b => b.TeamName == SelectedBot.TeamName);
            _botInformations.Remove(botToRemove);

            Logs.Add("Bot removed");
        }

        private void StartSimulation()
        {
            var simulator = new SimulatorWindow();
            simulator.Show(new GameConfiguration
            {
                BotPathList = BotInformations.Select(b => b.Path).ToList(),
                BotsPerTeam = int.Parse(Configuration.BotsPerTeam),
                GameType = int.Parse(Configuration.GameType),
                MapPath = Configuration.MapPath,
                NumberOfTurns = int.Parse(Configuration.NumberOfTurns),
                Speed = int.Parse(Configuration.Speed)
            });
        }

        private bool IsAssemblyValid(string path)
        {
            return LoadBot(path) != null;
        }

        private string ExtractTeamName(string path)
        {
            return LoadBot(path).TeamName;
        }

        private Bot LoadBot(string path)
        {
            var assembly = Assembly.LoadFile(path);
            var type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Contains(Path.GetFileNameWithoutExtension(path)));

            if (type == null)
            {
                return null;
            }

            return Activator.CreateInstance(type) as Bot;
        }

        // Commands

        public RelayCommand AddMapCommand { get; private set; }
        public RelayCommand AddBotCommand { get; private set; }
        public RelayCommand RemoveBotCommand { get; private set; }
        public RelayCommand StartSimulationCommand { get; private set; }

        // Properties

        private ObservableCollection<BotInfoModel> _botInformations;
        public ObservableCollection<BotInfoModel> BotInformations
        {
            get { return _botInformations; }
            set { Set(() => BotInformations, ref _botInformations, value); }
        }

        private ObservableCollection<string> _logs;
        public ObservableCollection<string> Logs
        {
            get { return _logs; }
            set { Set(() => Logs, ref _logs, value); }
        }

        private BotInfoModel _selectedBot;
        public  BotInfoModel SelectedBot
        {
            get { return _selectedBot; }
            set { Set(() => SelectedBot, ref _selectedBot, value); }
        }

        public ConfigurationViewModel Configuration { get; set; }
    }
}
