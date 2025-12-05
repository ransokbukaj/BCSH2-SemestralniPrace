using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class HistoryLogViewModel : ObservableObject
    {
        private readonly HistoryLogRepository repository = new HistoryLogRepository();

        [ObservableProperty]
        private ObservableCollection<HistoryLog> historyLogs = new();

        [ObservableProperty]
        private HistoryLog selectedHistoryLog;

        public HistoryLogViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            HistoryLogs = new ObservableCollection<HistoryLog>(repository.GetList());
        }
    }
}
