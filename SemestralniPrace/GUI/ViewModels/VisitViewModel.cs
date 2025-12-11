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

        [ObservableProperty]
        private ObservableCollection<Visit> visits = new();

        [ObservableProperty]
        private Visit selectedVisit;

        [ObservableProperty]
        private ObservableCollection<VisitType> visitTypes;

        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitions = new();

        public VisitViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                Visits = new ObservableCollection<Visit>(_repository.GetList());
                VisitTypes = new ObservableCollection<VisitType>(counterRep.GetVisitTypes());
                Exhibitions = new ObservableCollection<Exhibition>(exhibitRep.GetList());
            }, "Načtení návštěv selhalo");
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
