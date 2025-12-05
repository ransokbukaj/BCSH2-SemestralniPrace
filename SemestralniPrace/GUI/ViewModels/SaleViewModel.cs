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
    public partial class SaleViewModel : ObservableObject
    {
        private readonly SaleRepository repository = new SaleRepository();

        [ObservableProperty]
        private ObservableCollection<Sale> sales = new();

        [ObservableProperty]
        private Sale selectedSale;

        public SaleViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Sales = new ObservableCollection<Sale>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedSale = new Sale();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSale == null)
                return;

            repository.SaveItem(SelectedSale);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedSale == null || SelectedSale.Id == 0)
                return;

            repository.DeleteItem(SelectedSale.Id);
            Load();
        }
    }
}
