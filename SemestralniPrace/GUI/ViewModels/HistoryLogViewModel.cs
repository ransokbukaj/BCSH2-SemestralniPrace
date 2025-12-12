using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Helpers;
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

        //List pro načtení dat
        private List<HistoryLog> _allHistoryLogs = new();

        //List pro zobrazení dat
        [ObservableProperty]
        private ObservableCollection<HistoryLog> historyLogs = new();

        [ObservableProperty]
        private HistoryLog selectedHistoryLog;


        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        public HistoryLogViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allHistoryLogs = repository.GetList();
                ApplyFilter();
            }, "Načtení historie selhalo");
        }

        /// <summary>
        /// Metoda pro filtrování obsahu historie podle názvu uživatele, operace a názvu tabulky.
        /// </summary>
        private void ApplyFilter()
        {
            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                HistoryLogs = new ObservableCollection<HistoryLog>(_allHistoryLogs);
                return;
            }

            var lower = text.ToLowerInvariant();

            var filtered = _allHistoryLogs
                .Where(h =>
                    (!string.IsNullOrWhiteSpace(h.Username) && h.Username.ToLowerInvariant().Contains(lower)) ||
                    (!string.IsNullOrWhiteSpace(h.TableName) && h.TableName.ToLowerInvariant().Contains(lower)) ||
                    (!string.IsNullOrWhiteSpace(h.TypeOfOperation) && h.TypeOfOperation.ToLowerInvariant().Contains(lower)) 
                )
                .ToList();

            HistoryLogs = new ObservableCollection<HistoryLog>(filtered);
        }
    }
}

