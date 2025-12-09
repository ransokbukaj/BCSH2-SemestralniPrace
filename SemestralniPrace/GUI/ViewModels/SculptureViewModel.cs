using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
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
        private readonly CounterRepository counterRep = new CounterRepository();

        [ObservableProperty]
        private ObservableCollection<Sculpture> sculptures = new();

        [ObservableProperty]
        private ObservableCollection<Counter> materials = new();


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
            Materials = new ObservableCollection<Counter>(counterRep.GetMaterials());
        }

        [RelayCommand]
        private void New()
        {
            SelectedSculpture = new Sculpture()
            {
                Material = new Counter()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSculpture == null)
                return;


            if(Materials != null && SelectedSculpture != null)
            {
                SelectedSculpture.Material = Materials.FirstOrDefault(p => p.Id == SelectedSculpture.Material.Id);
            }

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
    }
}
