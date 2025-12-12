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
        private readonly ArtistRepository repository = new ArtistRepository();
        private readonly ArtPieceRepository artRepo = new ArtPieceRepository();
        private readonly UtilityRepository homeRep = new UtilityRepository();


        private List<Artist> _allArtists = new();
        [ObservableProperty]
        private ObservableCollection<Artist> artists = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> artPieces = new();

        [ObservableProperty]
        private Artist selectedArtist;

        [ObservableProperty]
        private ArtistStatistics stat;

        [ObservableProperty]
        private MentorBranchStatistics mentorBranch;

        [ObservableProperty]
        private MostSuccesfulMentore mostSuccesfulMentore;

        [ObservableProperty]
        private string searchText;
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
                    ArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListByArtistId(SelectedArtist.Id));
                    
                }, "Načtení děl umělce selhalo");

                ErrorHandler.SafeExecute(() =>
                {
                    Stat = homeRep.GetArtistStatistic(SelectedArtist.Id);
                    MentorBranch = homeRep.GetMentorBranchStatics(SelectedArtist.Id);
                    MostSuccesfulMentore = homeRep.GetMostSuccesfulMentore(SelectedArtist.Id);
                }, "Načtení výpisu informací");
            }
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allArtists = repository.GetList();
                //Artists = new ObservableCollection<Artist>(list);
                ApplyFilter();
                Stat = null;
            }, "Načtení umělců selhalo");
            ApplyFilter();
        }

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (_allArtists == null)
                return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // žádný filtr → zobraz vše
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

            // pokud po filtrování nic není → odznač detail
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

