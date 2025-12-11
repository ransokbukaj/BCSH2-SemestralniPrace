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
    public partial class ArtistViewModel : ObservableObject
    {
        private readonly ArtistRepository repository = new ArtistRepository();
        private readonly ArtPieceRepository artRepo = new ArtPieceRepository();

        [ObservableProperty]
        private ObservableCollection<Artist> artists = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> artPieces = new();

        [ObservableProperty]
        private Artist selectedArtist;

        public ArtistViewModel()
        {
            Load();
        }

        partial void OnSelectedArtistChanged(Artist value)
        {
            if (SelectedArtist != null)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    ArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListByArtistId(SelectedArtist.Id));
                }, "Načtení děl umělce selhalo");
            }
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                var list = repository.GetList();
                Artists = new ObservableCollection<Artist>(list);
            }, "Načtení umělců selhalo");
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

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedArtist.FirstName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Jméno umělce nesmí být prázdné");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedArtist.LastName))
                {
                    ErrorHandler.ShowError("Validační chyba", "Příjmení umělce nesmí být prázdné");
                    return;
                }

                if (SelectedArtist.DateOfBirth == DateTime.MinValue)
                {
                    ErrorHandler.ShowError("Validační chyba", "Datum narození musí být vyplněno");
                    return;
                }

                if (SelectedArtist.DateOfBirth > DateTime.Now)
                {
                    ErrorHandler.ShowError("Validační chyba", "Datum narození nemůže být v budoucnosti");
                    return;
                }

                // Validace data úmrtí (pokud je vyplněno)
                if (SelectedArtist.DateOfDeath != DateTime.MinValue)
                {
                    if (SelectedArtist.DateOfDeath < SelectedArtist.DateOfBirth)
                    {
                        ErrorHandler.ShowError("Validační chyba", "Datum úmrtí nemůže být před datem narození");
                        return;
                    }

                    if (SelectedArtist.DateOfDeath > DateTime.Now)
                    {
                        ErrorHandler.ShowError("Validační chyba", "Datum úmrtí nemůže být v budoucnosti");
                        return;
                    }
                }

                repository.SaveItem(SelectedArtist);
                Load();
            }, "Uložení umělce selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedArtist == null || SelectedArtist.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedArtist.Id);
                Load();
            }, "Smazání umělce selhalo. Umělec má pravděpodobně přiřazená díla.");
        }
    }
}

