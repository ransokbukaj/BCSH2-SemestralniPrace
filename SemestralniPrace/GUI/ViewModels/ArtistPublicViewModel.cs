using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using Entities.Home;
using GUI.Helpers;
using System.Collections.ObjectModel;

namespace GUI.ViewModels
{
    public partial class ArtistPublicViewModel : ObservableObject
    {
        private readonly ArtistRepository artistRepository = new ArtistRepository();
        private readonly ArtPieceRepository artPieceRepository = new ArtPieceRepository();
        private readonly UtilityRepository utilityRepository = new UtilityRepository();

        //List pro skutečné načítání dat
        private List<Artist> _allArtists = new();
        //List pro zobrazování načtených dat
        [ObservableProperty]
        private ObservableCollection<Artist> artists = new();

        //List pro zobrazování uměleckých děl
        [ObservableProperty]
        private ObservableCollection<ArtPiece> artPieces = new();

        [ObservableProperty]
        private Artist selectedArtist;

        //Statistika umělce
        [ObservableProperty]
        private ArtistStatistics stat;

        //Informace o linii umělce
        [ObservableProperty]
        private MentorBranchStatistics mentorBranch;

        //Informace o nejúspěšnějším umělci z linii podle počtu děl.
        [ObservableProperty]
        private MostSuccesfulMentore mostSuccesfulMentore;

        [ObservableProperty]
        private string searchText;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }


        public ArtistPublicViewModel()
        {
            Load();
        }

        partial void OnSelectedArtistChanged(Artist value)
        {
            if (SelectedArtist != null)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    ArtPieces = new ObservableCollection<ArtPiece>(artPieceRepository.GetListByArtistId(SelectedArtist.Id));
                    
                }, "Načtení děl umělce selhalo");

                ErrorHandler.SafeExecute(() =>
                {
                    Stat = utilityRepository.GetArtistStatistic(SelectedArtist.Id);
                    MentorBranch = utilityRepository.GetMentorBranchStatics(SelectedArtist.Id);
                    MostSuccesfulMentore = utilityRepository.GetMostSuccesfulMentore(SelectedArtist.Id);
                }, "Načtení výpisu informací");
            }
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allArtists = artistRepository.GetList();
                ApplyFilter();
                Stat = null;
            }, "Načtení umělců selhalo");
        }

       /// <summary>
       /// Metoda pro filtrování obsahu podle jména a příjmení umelců
       /// </summary>
        private void ApplyFilter()
        {
            if (_allArtists == null)
                return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Artists = new ObservableCollection<Artist>(_allArtists);
            }
            else
            {
                string filter = SearchText.Trim().ToLower();

                var filtered = _allArtists.Where(a =>
                    (!string.IsNullOrEmpty(a.FirstName) && a.FirstName.ToLower().Contains(filter)) ||
                    (!string.IsNullOrEmpty(a.LastName) && a.LastName.ToLower().Contains(filter))
                );

                Artists = new ObservableCollection<Artist>(filtered);
            }

            // pokud po filtrování nic není odznačá se detail
            if (!Artists.Contains(SelectedArtist))
            {
                SelectedArtist = null;
                ArtPieces.Clear();
                Stat = null;
                MentorBranch = null;
                MostSuccesfulMentore = null;
            }
        }
    }
}

