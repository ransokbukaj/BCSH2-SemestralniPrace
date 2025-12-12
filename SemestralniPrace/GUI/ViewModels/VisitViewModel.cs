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
        private readonly VisitRepository _repository = new VisitRepository();
        private readonly CounterRepository counterRep = new CounterRepository();
        private readonly ExhibitionRepository exhibitRep = new ExhibitionRepository();


        private List<Visit> _allVisits = new();
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
                _allVisits = _repository.GetList();
                //Visits = new ObservableCollection<Visit>(_repository.GetList());
                VisitTypes = new ObservableCollection<VisitType>(counterRep.GetVisitTypes());
                Exhibitions = new ObservableCollection<Exhibition>(exhibitRep.GetList());
                ApplyFilter();
            }, "Načtení návštěv selhalo");
        }

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

                // typ návštěvy
                var typeText = v.VisitType?.Name?.ToLowerInvariant() ?? "";

                // když chceš i id, můžeš přidat:
                // var idText = v.Id.ToString();

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

                _repository.SaveItem(SelectedVisit, SelectedVisit.ExhibitionCounter.Id);
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
                _repository.DeleteItem(SelectedVisit.Id);
                Load();
            }, "Smazání návštěvy selhalo");
        }
    }
}
