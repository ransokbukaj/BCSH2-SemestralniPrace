using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public partial class ExhibitionViewModel : ObservableObject
    {
        private readonly ExhibitionRepository repository = new ExhibitionRepository();
        private readonly ArtPieceRepository artRepo = new ArtPieceRepository();

        private List<Exhibition> _allExhibitions = new();
        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitions = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> availableArtPieces = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> exhibitionArtPieces = new();

        [ObservableProperty]
        private Exhibition selectedExhibition;

        [ObservableProperty]
        private ArtPiece selectedArtPieceToRemove;

        [ObservableProperty]
        private ArtPiece selectedArtPieceToAdd;

        [ObservableProperty]
        private string searchText;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedExhibitionChanged(Exhibition value)
        {
            if(SelectedExhibition != null)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    ExhibitionArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListByExhibitionId(SelectedExhibition.Id));
                    AvailableArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListInStorage());
                }, "Načtení děl selhalo");
            }
        }

        [RelayCommand]
        private void AddArtPiece()
        {
            if (SelectedExhibition == null || SelectedExhibition.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte výstavu");
                return;
            }

            if (SelectedArtPieceToAdd == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte dílo k přidání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                artRepo.AddArtPieceToExhibition(SelectedArtPieceToAdd.Id, SelectedExhibition.Id);
                ExhibitionArtPieces.Add(SelectedArtPieceToAdd);
                AvailableArtPieces.Remove(SelectedArtPieceToAdd);
                SelectedArtPieceToAdd = AvailableArtPieces.FirstOrDefault();
            }, "Přidání díla na výstavu selhalo");
        }

        [RelayCommand]
        private void RemoveArtPiece()
        {
            if (SelectedExhibition == null || SelectedExhibition.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte výstavu");
                return;
            }
            if (SelectedArtPieceToRemove == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte dílo k odebrání");
                return;
            }
            ErrorHandler.SafeExecute(() =>
            {
                artRepo.RemoveArtPieceFromExhibition(SelectedArtPieceToRemove.Id, SelectedExhibition.Id);
                AvailableArtPieces.Add(SelectedArtPieceToRemove);
                ExhibitionArtPieces.Remove(SelectedArtPieceToRemove);
                SelectedArtPieceToRemove = ExhibitionArtPieces.FirstOrDefault();
            }, "Odebrání díla z výstavy selhalo");
        }

        public ExhibitionViewModel()
        {
            Load();
            ApplyFilter();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allExhibitions = repository.GetList();
                //Exhibitions = new ObservableCollection<Exhibition>(repository.GetList());
                ApplyFilter();
            }, "Načtení výstav selhalo");
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Exhibitions = new ObservableCollection<Exhibition>(_allExhibitions);
            }
            else
            {
                var text = SearchText.Trim().ToLower();

                var filtered = _allExhibitions
                    .Where(e =>
                        e.Name != null &&
                        e.Name.ToLower().Contains(text))
                    .ToList();

                Exhibitions = new ObservableCollection<Exhibition>(filtered);
            }
        }

        [RelayCommand]
        private void New()
        {
            SelectedExhibition = new Exhibition();
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedExhibition == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedExhibition.Name))
                {
                    ErrorHandler.ShowError("Validační chyba", "Název výstavy nesmí být prázdný");
                    return;
                }

                if (string.IsNullOrWhiteSpace(SelectedExhibition.Description))
                {
                    ErrorHandler.ShowError("Validační chyba", "Popis výstavy nesmí být prázdný");
                    return;
                }

                if (SelectedExhibition.From > SelectedExhibition.To)
                {
                    ErrorHandler.ShowError("Validační chyba", "Datum začátku musí být před datem konce");
                    return;
                }

                repository.SaveItem(SelectedExhibition);
                Load();
            }, "Uložení výstavy selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedExhibition == null || SelectedExhibition.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedExhibition.Id);
                Load();
            }, "Smazání výstavy selhalo");
        }
    }
}
