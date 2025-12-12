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
        private readonly ExhibitionRepository exhibitionRepository = new ExhibitionRepository();
        private readonly ArtPieceRepository artPieceRepository = new ArtPieceRepository();

        //List pro načzení výstav
        private List<Exhibition> _allExhibitions = new();
        //List pro zobrazení výstav
        [ObservableProperty]
        private ObservableCollection<Exhibition> exhibitions = new();

        //List pro zobrazení dostupných děl
        [ObservableProperty]
        private ObservableCollection<ArtPiece> availableArtPieces = new();

        //List pro zobrazení děl na výstavě
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
                    ExhibitionArtPieces = new ObservableCollection<ArtPiece>(artPieceRepository.GetListByExhibitionId(SelectedExhibition.Id));
                    AvailableArtPieces = new ObservableCollection<ArtPiece>(artPieceRepository.GetListInStorage());
                }, "Načtení děl selhalo");
            }
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
                _allExhibitions = exhibitionRepository.GetList();
                
                ApplyFilter();
            }, "Načtení výstav selhalo");
        }


        /// <summary>
        /// Metoda pro filtrování obsahu podle jména výstavy
        /// </summary>
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
                artPieceRepository.AddArtPieceToExhibition(SelectedArtPieceToAdd.Id, SelectedExhibition.Id);
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
                artPieceRepository.RemoveArtPieceFromExhibition(SelectedArtPieceToRemove.Id, SelectedExhibition.Id);
                AvailableArtPieces.Add(SelectedArtPieceToRemove);
                ExhibitionArtPieces.Remove(SelectedArtPieceToRemove);
                SelectedArtPieceToRemove = ExhibitionArtPieces.FirstOrDefault();
            }, "Odebrání díla z výstavy selhalo");
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

                exhibitionRepository.SaveItem(SelectedExhibition);
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
                exhibitionRepository.DeleteItem(SelectedExhibition.Id);
                Load();
            }, "Smazání výstavy selhalo");
        }
    }
}
