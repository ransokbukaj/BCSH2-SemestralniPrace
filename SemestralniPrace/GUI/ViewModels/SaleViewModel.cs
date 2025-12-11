using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
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


        partial void OnSelectedSaleChanged(Sale value)
        {
            if (SelectedSale != null)
            {
                SaleArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListBySaleId(SelectedSale.Id));
                AvailableArtPieces = new ObservableCollection<ArtPiece>(artRepo.GetListUnsold());

            }
        }

        [RelayCommand]
        private void AddArtPiece()
        {
            if (SelectedSale != null && SelectedArtPieceToAdd != null)
            {
                artRepo.AddArtPieceToSale(SelectedArtPieceToAdd.Id, SelectedSale.Id);

                SaleArtPieces.Add(SelectedArtPieceToAdd);
                AvailableArtPieces.Remove(SelectedArtPieceToAdd);

                SelectedArtPieceToAdd = AvailableArtPieces.FirstOrDefault();
            }
        }
        [RelayCommand]
        private void RemoveArtPiece()
        {
            if (SelectedSale != null && SelectedArtPieceToRemove != null)
            {
                artRepo.RemoveArtPieceFromSale(SelectedArtPieceToRemove.Id);

                AvailableArtPieces.Add(SelectedArtPieceToRemove);
                SaleArtPieces.Remove(SelectedArtPieceToRemove);


                SelectedArtPieceToRemove = SaleArtPieces.FirstOrDefault();
            }
        }





        public SaleViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Sales = new ObservableCollection<Sale>(repository.GetList());
            TypesOfPayment = new ObservableCollection<Counter>(counterRep.GetPaymentMethods());
            Buyers = new ObservableCollection<Buyer>(buyerRep.GetList());
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
            if (SelectedSale == null || SelectedSale.Buyer.Id == 0 || SelectedSale.TypeOfPayment.Id == 0)
                return;

            if(SelectedSale != null && TypesOfPayment != null)
            {
                SelectedSale.TypeOfPayment = TypesOfPayment.FirstOrDefault(p => p.Id == SelectedSale.TypeOfPayment.Id);
            }

            repository.SaveItem(SelectedSale);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedSale == null || SelectedSale.Id == 0)
                return;

            repository.DeleteItem(SelectedSale.Id);
            Load();
        }
    }
}
