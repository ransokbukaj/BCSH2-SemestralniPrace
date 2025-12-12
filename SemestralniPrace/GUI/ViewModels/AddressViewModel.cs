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
    public partial class AddressViewModel : ObservableObject
    {
        private readonly AddressRepository adressRepository = new AddressRepository();
        private readonly PostRepository postRepository = new PostRepository();

        //List do kterého se data nahrávají
        private List<Address> _allAddresses = new();

        //List, který data zobrazuje
        [ObservableProperty]
        private ObservableCollection<Address> addresses = new();

        [ObservableProperty]
        private Address selectedAddress;

        [ObservableProperty]
        private Post selectedPost;

        public ObservableCollection<Post> Posts { get; set; }

        [ObservableProperty]
        private string searchText;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        public AddressViewModel()
        {
            Load();
        }


        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allAddresses = adressRepository.GetList();
                Addresses = new ObservableCollection<Address>(_allAddresses);
                Posts = new ObservableCollection<Post>(postRepository.GetList());
            }, "Načtení adres selhalo");
            ApplyFilter();
        }
        /// <summary>
        /// Metoda pro filtrování obsahu podle: názvu ulice, čísla domu, čísla ulice, města a PSČ
        /// </summary>
        private void ApplyFilter()
        {
            if (_allAddresses == null) return;

            var text = (SearchText ?? "").Trim();

            IEnumerable<Address> filtered = _allAddresses;

            if (!string.IsNullOrWhiteSpace(text))
            {
                var t = text.ToLowerInvariant();

                filtered = _allAddresses.Where(a =>
                    (a.Street ?? "").ToLowerInvariant().Contains(t) ||
                    (a.HouseNumber ?? "").ToLowerInvariant().Contains(t) ||
                    (a.StreetNumber ?? "").ToLowerInvariant().Contains(t) ||
                    (a.Post?.City ?? "").ToLowerInvariant().Contains(t) ||
                    (a.Post?.PSC ?? "").ToLowerInvariant().Contains(t)
                );
            }

            Addresses = new ObservableCollection<Address>(filtered);

            // když se po filtrování ztratí vybraný záznam, tak vyber první
            if (SelectedAddress != null && !Addresses.Contains(SelectedAddress))
                SelectedAddress = Addresses.FirstOrDefault();
        }


        [RelayCommand]
        private void New()
        {
            SelectedAddress = new Address()
            {
                Post = new Post()
            }; 
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedAddress == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedAddress.Street))
                {
                    ErrorHandler.ShowError("Validační chyba", "Ulice nesmí být prázdná");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedAddress.HouseNumber))
                {
                    ErrorHandler.ShowError("Validační chyba", "Číslo popisné nesmí být prázdné");
                    return;
                }

                if (SelectedAddress.Post == null || SelectedAddress.Post.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat poštu");
                    return;
                }

                if (Posts != null)
                {
                    SelectedAddress.Post = Posts.FirstOrDefault(p => p.Id == SelectedAddress.Post.Id);
                }

                adressRepository.SaveItem(SelectedAddress);
                Load();
            }, "Uložení adresy selhalo");
        }


        [RelayCommand]
        private void Delete()
        {
            if (SelectedAddress == null || SelectedAddress.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                adressRepository.DeleteItem(SelectedAddress.Id);
                Load();
            }, "Smazání adresy selhalo");
        }
    }
}
