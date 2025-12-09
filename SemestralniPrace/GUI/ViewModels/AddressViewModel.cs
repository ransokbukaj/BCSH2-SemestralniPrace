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

        public ObservableCollection<Post> Posts { get; set; }   // seznam všech PSÈ

       

      

        public AddressViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Addresses = new ObservableCollection<Address>(repository.GetList());
            Posts = new ObservableCollection<Post>(postRepository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedAddress = new Address()
            {
                Post = new Post()   // <<< Tohle je klíèové!
            }; 
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedAddress == null)
                return;

            if (Posts != null)
            {
                // sjednotit instanci Post podle Id
                SelectedAddress.Post = Posts.FirstOrDefault(p => p.Id == SelectedAddress.Post.Id);
            }

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
