using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class BuyerViewModel : ObservableObject
    {
        private readonly BuyerRepository repository = new BuyerRepository();

        [ObservableProperty]
        private ObservableCollection<Buyer> buyers = new();

        [ObservableProperty]
        private Buyer selectedBuyer;

        public BuyerViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            var list = repository.GetList();
            Buyers = new ObservableCollection<Buyer>(list);
        }

        [RelayCommand]
        private void New()
        {
            SelectedBuyer = new Buyer();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedBuyer == null)
                return;

            repository.SaveItem(SelectedBuyer);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedBuyer == null || SelectedBuyer.Id == 0)
                return;

            repository.DeleteItem(SelectedBuyer.Id);
            Load();
        }
    }
}
