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
    public partial class AddressViewModel : ObservableObject
    {
        private readonly AddressRepository repository = new AddressRepository();

        [ObservableProperty]
        private ObservableCollection<Address> addresses = new();

        [ObservableProperty]
        private Address selectedAddress;

        public AddressViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Addresses = new ObservableCollection<Address>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedAddress = new Address();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedAddress == null)
                return;

            repository.SaveItem(SelectedAddress);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedAddress == null || SelectedAddress.Id == 0)
                return;

            repository.DeleteItem(SelectedAddress.Id);
            Load();
        }
    }
}
