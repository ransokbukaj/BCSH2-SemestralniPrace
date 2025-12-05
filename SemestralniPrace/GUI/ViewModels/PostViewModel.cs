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
    public partial class PostViewModel : ObservableObject
    {
        private readonly PostRepository repository = new PostRepository();

        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private Post selectedPost;

        public PostViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Posts = new ObservableCollection<Post>(repository.GetList());
        }

        [RelayCommand]
        private void New()
        {
            SelectedPost = new Post();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedPost == null)
                return;

            repository.SaveItem(SelectedPost);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedPost == null || SelectedPost.Id == 0)
                return;

            repository.DeleteItem(SelectedPost.Id);
            Load();
        }
    }
}
