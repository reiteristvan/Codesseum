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

            _botInformations = new ObservableCollection<BotInfo>();
        }

        public void AddBot()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "C# assemblies (*.dll)|*.dll";

            if (dialog.ShowDialog() == true)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    BotInformations.Add(new BotInfo
                    {
                        Path = fileName
                    });
                }
            }
        }

        public void RemoveBot()
        {
            
        }

        private bool IsAssemblyValid(string path)
        {
            var assembly = Assembly.LoadFile(path);
            var type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Contains(Path.GetFileNameWithoutExtension(path)));
            var bot = Activator.CreateInstance(type) as Bot;

            return bot != null;
        }

        private string ExtractTeamName(string path)
        {
            var assembly = Assembly.LoadFile(path);
            var type = assembly.GetTypes()
                .FirstOrDefault(t => t.Name.Contains(Path.GetFileNameWithoutExtension(path)));
            var bot = Activator.CreateInstance(type) as Bot;

            return bot.TeamName;
        }

        // Commands

        public RelayCommand AddBotCommand { get; private set; }
        public RelayCommand RemoveBotCommand { get; private set; }

        // Properties

        private ObservableCollection<BotInfo> _botInformations;

        public ObservableCollection<BotInfo> BotInformations
        {
            get { return _botInformations; }
            set { Set(() => BotInformations, ref _botInformations, value); }
        }
    }
}
