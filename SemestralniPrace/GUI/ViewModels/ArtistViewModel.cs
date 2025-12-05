using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities.Data;

using DatabaseAccess;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace GUI.ViewModels
{
    public partial class ArtistViewModel : ObservableObject
    {

        private readonly ArtistRepository repository = new ArtistRepository();

        [ObservableProperty]
        private ObservableCollection<Artist> artists = new();

        [ObservableProperty]
        private Artist selectedArtist;

        public ArtistViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            var list = repository.GetList();
            Artists = new ObservableCollection<Artist>(list);
        }

        [RelayCommand]
        private void New()
        {
            SelectedArtist = new Artist();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedArtist == null)
                return;

            repository.SaveItem(SelectedArtist);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedArtist == null || SelectedArtist.Id == 0)
                return;

            repository.DeleteItem(SelectedArtist.Id);
            Load();
        }

        private ArtistRepository artistRepository;

    }


}

