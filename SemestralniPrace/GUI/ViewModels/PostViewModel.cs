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
        private readonly PostRepository postRepository = new PostRepository();

        //List pro načtení dat
        private List<Post> _allPosts = new();
        //List pro zobrazení dat
        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private Post selectedPost;

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        public PostViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allPosts = postRepository.GetList();
                
                ApplyFilter();
            }, "Načtení PSČ selhalo");
        }
        /// <summary>
        /// Metoda pro filtrování obsahu podle názvu mětsa a PSČ.
        /// </summary>
        private void ApplyFilter()
        {
            if (_allPosts == null) return;

            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Posts = new ObservableCollection<Post>(_allPosts);
                return;
            }

            var lower = text.ToLowerInvariant();

            var filtered = _allPosts
                .Where(p =>
                    (!string.IsNullOrWhiteSpace(p.City) && p.City.ToLowerInvariant().Contains(lower)) ||
                    (!string.IsNullOrWhiteSpace(p.PSC) && p.PSC.ToLowerInvariant().Contains(lower)) ||
                    // hledání i bez mezery: "12345" najde i "123 45"
                    (!string.IsNullOrWhiteSpace(p.PSC) && p.PSC.Replace(" ", "").Contains(text.Replace(" ", "")))
                )
                .ToList();

            Posts = new ObservableCollection<Post>(filtered);
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

                postRepository.SaveItem(SelectedPost);
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
                postRepository.DeleteItem(SelectedPost.Id);
                Load();
            }, "Smazání PSČ selhalo. PSČ je pravděpodobně používáno v adresách.");
        }
    }
}
