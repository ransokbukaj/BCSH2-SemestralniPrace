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
    public partial class VisitViewModel : ObservableObject
    {
        private readonly VisitRepository visitRepository = new VisitRepository();
        private readonly CounterRepository counterRepository = new CounterRepository();
        private readonly ExhibitionRepository exhibitRepository = new ExhibitionRepository();

        //List pro načtení všech návštěv
        private List<Visit> _allVisits = new();
        //List pro zobrazení návštěv
        [ObservableProperty]
        private ObservableCollection<Visit> visits = new();

        [ObservableProperty]
        private Visit selectedVisit;

        [ObservableProperty]
        private ObservableCollection<VisitType> visitTypes;

        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitions = new();

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }


        public VisitViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allVisits = visitRepository.GetList();
                
                VisitTypes = new ObservableCollection<VisitType>(counterRepository.GetVisitTypes());
                Exhibitions = new ObservableCollection<Exhibition>(exhibitRepository.GetList());
                ApplyFilter();
            }, "Načtení návštěv selhalo");
        }

        /// <summary>
        /// Metoda pro filtrování obsahu podle data a druhu návštevy
        /// </summary>
        private void ApplyFilter()
        {
            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Visits = new ObservableCollection<Visit>(_allVisits);
                return;
            }

            var lower = text.ToLowerInvariant();

            var filtered = _allVisits.Where(v =>
            {
                // datum jako text (dd.MM.yyyy)
                var dateText = v.DateOfVisit.ToString("dd.MM.yyyy").ToLowerInvariant();

                // drug návštěvy
                var typeText = v.VisitType?.Name?.ToLowerInvariant() ?? "";

                return dateText.Contains(lower)
                       || typeText.Contains(lower);
            }).ToList();

            Visits = new ObservableCollection<Visit>(filtered);
        }

        [RelayCommand]
        private void New()
        {
            SelectedVisit = new Visit()
            {
                VisitType = new VisitType()
                {
                    Id = 0
                },
                ExhibitionCounter = new Counter()
                {
                    Id = 0
                }
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedVisit == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (SelectedVisit.ExhibitionCounter == null || SelectedVisit.ExhibitionCounter.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat výstavu");
                    return;
                }

                if (SelectedVisit.VisitType == null || SelectedVisit.VisitType.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat typ návštěvy");
                    return;
                }

                visitRepository.SaveItem(SelectedVisit, SelectedVisit.ExhibitionCounter.Id);
                Load();
            }, "Uložení návštěvy selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedVisit == null || SelectedVisit.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                visitRepository.DeleteItem(SelectedVisit.Id);
                Load();
            }, "Smazání návštěvy selhalo");
        }
    }
}
