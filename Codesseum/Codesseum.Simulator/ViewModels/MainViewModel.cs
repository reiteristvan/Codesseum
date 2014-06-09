using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
            AddBotCommand = new RelayCommand(AddBot);
            RemoveBotCommand = new RelayCommand(RemoveBot);
            StartSimulationCommand = new RelayCommand(StartSimulation);

            _botInformations = new ObservableCollection<BotInfo>();
            _logs = new ObservableCollection<string>();
        }

        public void AddBot()
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

                    BotInformations.Add(new BotInfo
                    {
                        Path = fileName,
                        TeamName = ExtractTeamName(fileName)
                    });
                }
            }
        }

        public void RemoveBot()
        {
            
        }

        public void StartSimulation()
        {
            
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

        public RelayCommand AddBotCommand { get; private set; }
        public RelayCommand RemoveBotCommand { get; private set; }
        public RelayCommand StartSimulationCommand { get; private set; }

        // Properties

        private ObservableCollection<BotInfo> _botInformations;
        public ObservableCollection<BotInfo> BotInformations
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
    }
}
