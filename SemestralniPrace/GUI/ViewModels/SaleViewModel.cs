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
    public partial class SaleViewModel : ObservableObject
    {
        private readonly SaleRepository repository = new SaleRepository();
        private readonly CounterRepository counterRep = new CounterRepository();
        private readonly BuyerRepository buyerRep = new BuyerRepository();
        private readonly ArtPieceRepository artRepo = new ArtPieceRepository();


        private List<Sale> _allSales = new();
        [ObservableProperty]
        private ObservableCollection<Sale> sales = new();

        [ObservableProperty]
        private ObservableCollection<Counter> typesOfPayment = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> availableArtPieces = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> saleArtPieces = new();

        [ObservableProperty]
        private ObservableCollection<Buyer> buyers = new();

        [ObservableProperty]
        private Sale selectedSale;

        [ObservableProperty]
        private ArtPiece selectedArtPieceToRemove;

        [ObservableProperty]
        private ArtPiece selectedArtPieceToAdd;

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedSaleChanged(Sale value)
        {
            ErrorHandler.SafeExecute(() =>
            {
                if (SelectedSale != null)
                {
                    SaleArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListBySaleId(SelectedSale.Id));
                    AvailableArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListUnsold());
                }
            }, "Načtení uměleckých děl pro prodej selhalo");
        }

        [RelayCommand]
        private void AddArtPiece()
        {
            if (SelectedSale == null || SelectedArtPieceToAdd == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                // Kontrola, zda je prodej již uložen
                if (SelectedSale.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Před přidáním uměleckého díla musíte nejprve uložit prodej");
                    return;
                }

                artRepo.AddArtPieceToSale(SelectedArtPieceToAdd.Id, SelectedSale.Id);

                SaleArtPieces.Add(SelectedArtPieceToAdd);
                AvailableArtPieces.Remove(SelectedArtPieceToAdd);

                SelectedArtPieceToAdd = AvailableArtPieces.FirstOrDefault();
            }, "Přidání uměleckého díla do prodeje selhalo");
        }

        [RelayCommand]
        private void RemoveArtPiece()
        {
            if (SelectedSale == null || SelectedArtPieceToRemove == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                artRepo.RemoveArtPieceFromSale(SelectedArtPieceToRemove.Id);

                AvailableArtPieces.Add(SelectedArtPieceToRemove);
                SaleArtPieces.Remove(SelectedArtPieceToRemove);

                SelectedArtPieceToRemove = SaleArtPieces.FirstOrDefault();
            }, "Odebrání uměleckého díla z prodeje selhalo");
        }

        public SaleViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allSales = repository.GetList();
                //Sales = new ObservableCollection<Sale>(repository.GetList());
                TypesOfPayment = new ObservableCollection<Counter>(counterRep.GetPaymentMethods());
                Buyers = new ObservableCollection<Buyer>(buyerRep.GetList());
                ApplyFilter();
            }, "Načtení prodejů selhalo");
        }

        private void ApplyFilter()
        {
            if (_allSales == null)
                return;

            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Sales = new ObservableCollection<Sale>(_allSales);
                return;
            }

            var lower = text.ToLowerInvariant();

            var filtered = _allSales.Where(s =>
            {
                // cena
                var price = s.Price.ToString().ToLowerInvariant();

                // datum (víc formátů, ať se to dobře hledá)
                var date = s.DateOfSale.ToString("d").ToLowerInvariant();
                var dateIso = s.DateOfSale.ToString("yyyy-MM-dd").ToLowerInvariant();

                // karta / účet (pozor na null)
                var card = (s.CardNumber ?? "").ToLowerInvariant();
                var account = (s.AccountNumber ?? "").ToLowerInvariant();

                return price.Contains(lower)
                    || date.Contains(lower)
                    || dateIso.Contains(lower)
                    || card.Contains(lower)
                    || account.Contains(lower);
            }).ToList();

            Sales = new ObservableCollection<Sale>(filtered);
        }

        [RelayCommand]
        private void New()
        {
            SelectedSale = new Sale()
            {
                TypeOfPayment = new Counter()
                {
                    Id = 0
                },
                Buyer = new Buyer()
                {
                    Id = 0
                }
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSale == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (SelectedSale.Buyer == null || SelectedSale.Buyer.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat kupce");
                    return;
                }

                if (SelectedSale.TypeOfPayment == null || SelectedSale.TypeOfPayment.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat typ platby");
                    return;
                }

                if (SelectedSale != null && TypesOfPayment != null)
                {
                    SelectedSale.TypeOfPayment = TypesOfPayment.FirstOrDefault(p => p.Id == SelectedSale.TypeOfPayment.Id);
                }

                repository.SaveItem(SelectedSale);
                Load();
            }, "Uložení prodeje selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedSale == null || SelectedSale.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedSale.Id);
                Load();
            }, "Smazání prodeje selhalo. Prodej má pravděpodobně přiřazená umělecká díla.");
        }
    }
}
