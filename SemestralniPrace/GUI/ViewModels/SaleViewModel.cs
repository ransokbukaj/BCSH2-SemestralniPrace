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
    public partial class SaleViewModel : ObservableObject
    {
        private readonly SaleRepository repository = new SaleRepository();
        private readonly CounterRepository counterRep = new CounterRepository();

        [ObservableProperty]
        private ObservableCollection<Sale> sales = new();
        [ObservableProperty]
        private ObservableCollection<Counter> typesOfPayment = new();

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
            TypesOfPayment = new ObservableCollection<Counter>(counterRep.GetPaymentMethods());
        }

        [RelayCommand]
        private void New()
        {
            SelectedSale = new Sale() 
            {
                TypeOfPayment = new Counter()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSale == null)
                return;

            if(SelectedSale != null && TypesOfPayment != null)
            {
                SelectedSale.TypeOfPayment = TypesOfPayment.FirstOrDefault(p => p.Id == SelectedSale.TypeOfPayment.Id);
            }

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
