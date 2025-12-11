using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DatabaseAccess;
using Entities;
using GUI.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GUI.ViewModels
{
    public partial class SculptureViewModel : ObservableObject
    {
        private readonly SculptureRepository repository = new SculptureRepository();
        private readonly CounterRepository counterRep = new CounterRepository();
        private readonly AttachmentRepository attRep = new AttachmentRepository();
        private readonly ArtistRepository artistRep = new ArtistRepository();

        [ObservableProperty]
        private ObservableCollection<Sculpture> sculptures = new();

        [ObservableProperty]
        private ObservableCollection<Counter> materials = new();

        [ObservableProperty]
        private ObservableCollection<Attachment> attachments = new();

        [ObservableProperty]
        private ObservableCollection<Artist> authors = new();

        [ObservableProperty]
        private ObservableCollection<Artist> availableArtists = new();

        [ObservableProperty]
        private Sculpture selectedSculpture;

        [ObservableProperty]
        private Attachment selectedAttachment;

        [ObservableProperty]
        private ImageSource selectedImage;

        [ObservableProperty]
        private Artist selectedArtistToAdd;

        [ObservableProperty]
        private Artist selectedArtistToRemove;

        partial void OnSelectedSculptureChanged(Sculpture? oldValue, Sculpture newValue)
        {
            if (SelectedSculpture != null && SelectedSculpture.Id != 0)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    Attachments = new ObservableCollection<Attachment>(
                        attRep.GetListByArtPieceId(SelectedSculpture.Id));

                    if (Attachments.Count > 0)
                    {
                        SelectedAttachment = Attachments[0];
                        SelectedImage = AttachmentHelper.LoadImageSource(SelectedAttachment.File);
                    }
                    else
                    {
                        SelectedAttachment = null;
                        SelectedImage = null;
                    }

                    Authors = new ObservableCollection<Artist>(
                        artistRep.GetListByArtPieceId(SelectedSculpture.Id));

                    var assignedIds = new HashSet<int>(
                        artistRep.GetListByArtPieceId(SelectedSculpture.Id).Select(a => a.Id));

                    var coll = artistRep.GetList().Where(a => !assignedIds.Contains(a.Id)).ToList();
                    AvailableArtists = new ObservableCollection<Artist>(coll);
                }, "Načtení detailů sochy selhalo");
            }
        }

        partial void OnSelectedAttachmentChanged(Attachment value)
        {
            if (SelectedAttachment != null)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    SelectedImage = AttachmentHelper.LoadImageSource(SelectedAttachment.File);
                }, "Načtení obrázku selhalo");
            }
        }

        [RelayCommand]
        private void AddArtistToSculpture()
        {
            if (SelectedSculpture == null || SelectedSculpture.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte sochu");
                return;
            }

            if (SelectedArtistToAdd == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte umělce k přidání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                artistRep.AddArtistToArtPiece(SelectedArtistToAdd.Id, SelectedSculpture.Id);
                Authors.Add(SelectedArtistToAdd);
                AvailableArtists.Remove(SelectedArtistToAdd);

                SelectedArtistToAdd = AvailableArtists.FirstOrDefault();
            }, "Přidání autora selhalo");
        }

        [RelayCommand]
        private void RemoveArtistFromSculpture()
        {
            if (SelectedSculpture == null || SelectedSculpture.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte sochu");
                return;
            }

            if (SelectedArtistToRemove == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte autora k odebrání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                artistRep.RemoveArtistFromArtPiece(SelectedArtistToRemove.Id, SelectedSculpture.Id);
                AvailableArtists.Add(SelectedArtistToRemove);
                Authors.Remove(SelectedArtistToRemove);

                SelectedArtistToRemove = Authors.FirstOrDefault();
            }, "Odebrání autora selhalo");
        }

        [RelayCommand]
        private void PreviousImage()
        {
            if (SelectedAttachment != null)
            {
                int cur = Attachments.IndexOf(SelectedAttachment);
                if (cur == 0)
                {
                    SelectedAttachment = Attachments.Last();
                }
                else
                {
                    SelectedAttachment = Attachments[cur - 1];
                }
            }

        }

        [RelayCommand]
        private void NextImage()
        {
            if (SelectedAttachment != null)
            {
                int cur = Attachments.IndexOf(SelectedAttachment);
                if (cur == (Attachments.Count - 1))
                {
                    SelectedAttachment = Attachments.First();
                }
                else
                {
                    SelectedAttachment = Attachments[cur + 1];
                }
            }

        }

        [RelayCommand]
        private void AddImage()
        {
            if (SelectedSculpture == null || SelectedSculpture.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte sochu");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                var dlg = new OpenFileDialog
                {
                    Title = "Vyber obrázek",
                    Filter = "Obrázky|*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                    Multiselect = false
                };

                if (dlg.ShowDialog() == true)
                {
                    Attachment att = new Attachment
                    {
                        FileName = Path.GetFileName(dlg.FileName),
                        File = File.ReadAllBytes(dlg.FileName),
                        FileType = Path.GetExtension(dlg.FileName)
                    };

                    attRep.SaveItem(att, SelectedSculpture.Id);
                    Attachments.Add(att);
                    SelectedAttachment = att;
                }
            }, "Přidání obrázku selhalo");
        }

        [RelayCommand]
        private void RemoveCurrentImage()
        {
            if (SelectedAttachment == null)
            {
                ErrorHandler.ShowError("Chyba", "Není vybrán žádný obrázek");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                attRep.DeleteItem(SelectedAttachment.Id);

                SelectedImage = null;
                Attachments.Remove(SelectedAttachment);
                SelectedAttachment = Attachments.FirstOrDefault();
            }, "Smazání obrázku selhalo");
        }

        public SculptureViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                Sculptures = new ObservableCollection<Sculpture>(repository.GetList());
                Materials = new ObservableCollection<Counter>(counterRep.GetMaterials());
            }, "Načtení soch selhalo");
        }

        [RelayCommand]
        private void New()
        {
            SelectedAttachment = null;
            SelectedImage = null;
            SelectedSculpture = new Sculpture()
            {
                Material = new Counter()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedSculpture == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedSculpture.Name))
                {
                    ErrorHandler.ShowError("Validační chyba", "Název sochy nesmí být prázdný");
                    return;
                }

                if (SelectedSculpture.Height <= 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Výška musí být větší než 0");
                    return;
                }

                if (SelectedSculpture.Width <= 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Šířka musí být větší než 0");
                    return;
                }

                if (SelectedSculpture.Depth <= 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Hloubka musí být větší než 0");
                    return;
                }

                if (SelectedSculpture.Weight <= 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Váha musí být větší než 0");
                    return;
                }

                if (SelectedSculpture.Material == null || SelectedSculpture.Material.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat materiál");
                    return;
                }

                if (Materials != null && SelectedSculpture != null)
                {
                    SelectedSculpture.Material = Materials.FirstOrDefault(p => p.Id == SelectedSculpture.Material.Id);
                }

                repository.SaveItem(SelectedSculpture);
                Load();
            }, "Uložení sochy selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedSculpture == null || SelectedSculpture.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedSculpture.Id);
                Load();
            }, "Smazání sochy selhalo.");
        }
    }
}
