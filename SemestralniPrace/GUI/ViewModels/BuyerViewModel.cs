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
    public partial class BuyerViewModel : ObservableObject
    {
        private readonly BuyerRepository repository = new BuyerRepository();
        private readonly AddressRepository addressRepository = new AddressRepository();


        private List<Buyer> _allBuyers = new();
        [ObservableProperty]
        private ObservableCollection<Buyer> buyers = new();

        [ObservableProperty]
        private ObservableCollection<Address> addresses = new();

        [ObservableProperty]
        private Buyer selectedBuyer;

        [ObservableProperty]
        private string searchText;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        public BuyerViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allBuyers = repository.GetList();
                Buyers = new ObservableCollection<Buyer>(_allBuyers);
                Addresses = new ObservableCollection<Address>(addressRepository.GetList());
            }, "Načtení kupců selhalo");
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allBuyers == null)
                return;

            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Buyers = new ObservableCollection<Buyer>(_allBuyers);
                return;
            }

            text = text.ToLowerInvariant();

            var filtered = _allBuyers.Where(b =>
                (!string.IsNullOrWhiteSpace(b.FirstName) && b.FirstName.ToLowerInvariant().Contains(text)) ||
                (!string.IsNullOrWhiteSpace(b.LastName) && b.LastName.ToLowerInvariant().Contains(text)) ||
                (!string.IsNullOrWhiteSpace(b.PhoneNumber) && b.PhoneNumber.ToLowerInvariant().Contains(text)) ||
                (!string.IsNullOrWhiteSpace(b.Email) && b.Email.ToLowerInvariant().Contains(text))).ToList();

            Buyers = new ObservableCollection<Buyer>(filtered);

            if (SelectedBuyer != null && !filtered.Any(x => x.Id == SelectedBuyer.Id))
                SelectedBuyer = filtered.FirstOrDefault();
        }

        [RelayCommand]
        private void New()
        {
            SelectedBuyer = new Buyer()
            {
                Adress = new Address()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedBuyer == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedBuyer.FirstName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Jméno kupce nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedBuyer.LastName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Příjmení kupce nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedBuyer.PhoneNumber))
                {
                    ErrorHandler.ShowError("Validační chyba", "Telefonní číslo nesmí být prázdné");
                    return;
                }

                // Validace telefonního čísla (základní kontrola)
                if (SelectedBuyer.PhoneNumber.Length < 9)
                {
                    ErrorHandler.ShowError("Validační chyba", "Telefonní číslo musí mít alespoň 9 číslic");
                    return;
                }

                // Validace emailu (pokud je vyplněn)
                if (!string.IsNullOrWhiteSpace(SelectedBuyer.Email) && !SelectedBuyer.Email.Contains("@"))
                {
                    ErrorHandler.ShowError("Validační chyba", "Email není ve správném formátu");
                    return;
                }

                if (SelectedBuyer.Adress == null || SelectedBuyer.Adress.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat adresu");
                    return;
                }

                if (SelectedBuyer?.Adress != null && Addresses != null)
                {
                    SelectedBuyer.Adress = Addresses.FirstOrDefault(a => a.Id == SelectedBuyer.Adress.Id);
                }

                repository.SaveItem(SelectedBuyer);
                Load();
            }, "Uložení kupce selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedBuyer == null || SelectedBuyer.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedBuyer.Id);
                Load();
            }, "Smazání kupce selhalo. Kupec má pravděpodobně přiřazené prodeje.");
        }
    }
}
