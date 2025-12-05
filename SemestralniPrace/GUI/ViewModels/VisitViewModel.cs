using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Data;

using DatabaseAccess;

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

        [ObservableProperty]
        private ObservableCollection<Visit> visits = new();

        [ObservableProperty]
        private Visit selectedVisit;

        public VisitViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Visits = new ObservableCollection<Visit>(_repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedVisit = new Visit();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedVisit == null)
                return;

            _repository.SaveItem(SelectedVisit);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedVisit == null || SelectedVisit.Id == 0)
                return;

            _repository.DeleteItem(SelectedVisit.Id);
            Load();
        }

    public class VisitViewModel : ObservableObject
    {
        private VisitRepository visitRepository;
        private CounterRepository counterRepository;

    }
}
