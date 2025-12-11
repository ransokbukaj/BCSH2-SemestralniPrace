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
        private readonly AddressRepository repository = new AddressRepository();
        private readonly PostRepository postRepository = new PostRepository();

        [ObservableProperty]
        private ObservableCollection<Address> addresses = new();

        [ObservableProperty]
        private Address selectedAddress;

        [ObservableProperty]
        private Post selectedPost;

        public ObservableCollection<Post> Posts { get; set; }         

        public AddressViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                Addresses = new ObservableCollection<Address>(repository.GetList());
                Posts = new ObservableCollection<Post>(postRepository.GetList());
            }, "Načtení adres selhalo");
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

                repository.SaveItem(SelectedAddress);
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
                repository.DeleteItem(SelectedAddress.Id);
                Load();
            }, "Smazání adresy selhalo");
        }
    }
}
