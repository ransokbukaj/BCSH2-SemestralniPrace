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

    public partial class SculptureViewModel : ObservableObject
    {
        private readonly SculptureRepository repository = new SculptureRepository();

        [ObservableProperty]
        private ObservableCollection<Sculpture> sculptures = new();

        [ObservableProperty]
        private Sculpture selectedSculpture;

        public SculptureViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Sculptures = new ObservableCollection<Sculpture>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedSculpture = new Sculpture();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSculpture == null)
                return;

            repository.SaveItem(SelectedSculpture);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedSculpture == null || SelectedSculpture.Id == 0)
                return;

            repository.DeleteItem(SelectedSculpture.Id);
            Load();
        }

    public class SculptureViewModel : ObservableObject
    {
        private SculptureRepository sculptureRepository;
        private CounterRepository counterRepository;

    }
}
