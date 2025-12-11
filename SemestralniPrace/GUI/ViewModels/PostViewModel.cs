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
            ErrorHandler.SafeExecute(() =>
            {
                var list = repository.GetList();
                Posts = new ObservableCollection<Post>(list);
            }, "Načtení PSČ selhalo");
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

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedPost.PSC))
                {
                    ErrorHandler.ShowError("Validační chyba", "PSČ nesmí být prázdné");
                    return;
                }

                // PSČ musí mít 5 číslic (nebo 6 s mezerou: "123 45")
                var cleanedPSC = SelectedPost.PSC.Replace(" ", "");
                if (cleanedPSC.Length != 5)
                {
                    ErrorHandler.ShowError("Validační chyba", "PSČ musí mít 5 číslic (formát: 12345 nebo 123 45)");
                    return;
                }

                if (!int.TryParse(cleanedPSC, out _))
                {
                    ErrorHandler.ShowError("Validační chyba", "PSČ musí obsahovat pouze číslice");
                    return;
                }

                // Validace města
                if (string.IsNullOrWhiteSpace(SelectedPost.City))
                {
                    ErrorHandler.ShowError("Validační chyba", "Město nesmí být prázdné");
                    return;
                }

                repository.SaveItem(SelectedPost);
                Load();
            }, "Uložení PSČ selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedPost == null || SelectedPost.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedPost.Id);
                Load();
            }, "Smazání PSČ selhalo. PSČ je pravděpodobně používáno v adresách.");
        }
    }
}
