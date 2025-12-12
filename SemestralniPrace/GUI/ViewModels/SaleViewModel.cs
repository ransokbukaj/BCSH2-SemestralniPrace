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
        private readonly SaleRepository saleRepository = new SaleRepository();
        private readonly CounterRepository counterRepository = new CounterRepository();
        private readonly BuyerRepository buyerRepository = new BuyerRepository();
        private readonly ArtPieceRepository artPieceRepository = new ArtPieceRepository();

        //List pro načtení dat
        private List<Sale> _allSales = new();
        //List pro zobrazení prodejů
        [ObservableProperty]
        private ObservableCollection<Sale> sales = new();

        [ObservableProperty]
        private ObservableCollection<Counter> typesOfPayment = new();

        [ObservableProperty]
        private ObservableCollection<ArtPiece> availableArtPieces = new();

        //Zobrazení umeleckých děl v prodeji
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
                    SaleArtPieces = new ObservableCollection<ArtPiece>(artPieceRepository.GetListBySaleId(SelectedSale.Id));
                    AvailableArtPieces = new ObservableCollection<ArtPiece>(artPieceRepository.GetListUnsold());
                }
            }, "Načtení uměleckých děl pro prodej selhalo");
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
                _allSales = saleRepository.GetList();
                
                TypesOfPayment = new ObservableCollection<Counter>(counterRepository.GetPaymentMethods());
                Buyers = new ObservableCollection<Buyer>(buyerRepository.GetList());
                ApplyFilter();
            }, "Načtení prodejů selhalo");
        }

        /// <summary>
        /// Metoda pro filtrování prodejů podle ceny, data a karty/účtu
        /// </summary>
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
        private void AddArtPiece()
        {
            if (SelectedSale == null || SelectedArtPieceToAdd == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (SelectedSale.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Před přidáním uměleckého díla musíte nejprve uložit prodej");
                    return;
                }

                artPieceRepository.AddArtPieceToSale(SelectedArtPieceToAdd.Id, SelectedSale.Id);

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
                artPieceRepository.RemoveArtPieceFromSale(SelectedArtPieceToRemove.Id);

                AvailableArtPieces.Add(SelectedArtPieceToRemove);
                SaleArtPieces.Remove(SelectedArtPieceToRemove);

                SelectedArtPieceToRemove = SaleArtPieces.FirstOrDefault();
            }, "Odebrání uměleckého díla z prodeje selhalo");
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

                saleRepository.SaveItem(SelectedSale);
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
                saleRepository.DeleteItem(SelectedSale.Id);
                Load();
            }, "Smazání prodeje selhalo. Prodej má pravděpodobně přiřazená umělecká díla.");
        }
    }
}
