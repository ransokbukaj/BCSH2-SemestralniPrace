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
    public partial class PaintingViewModel : ObservableObject
    {
        private readonly PaintingRepository repository = new PaintingRepository();
        private readonly CounterRepository counterRep = new CounterRepository();
        private readonly AttachmentRepository attRep = new AttachmentRepository();
        private readonly ArtistRepository artistRep = new ArtistRepository();

        private List<Painting> _allPaintings = new();

        [ObservableProperty]
        private ObservableCollection<Painting> paintings = new();

        [ObservableProperty]
        private ObservableCollection<Counter> techniques = new();

        [ObservableProperty]
        private ObservableCollection<Counter> bases = new();

        [ObservableProperty]
        private ObservableCollection<Attachment> attachments = new();

        [ObservableProperty]
        private ObservableCollection<Artist> authors = new();

        [ObservableProperty]
        private ObservableCollection<Artist> availableArtists = new();

        [ObservableProperty]
        private Painting selectedPainting;

        [ObservableProperty]
        private Attachment selectedAttachment;

        [ObservableProperty]
        private ImageSource selectedImage;

        [ObservableProperty]
        private Artist selectedArtistToAdd;

        [ObservableProperty]
        private Artist selectedArtistToRemove;

        [ObservableProperty]
        private string searchText = string.Empty;

        partial void OnSearchTextChanged(string value)
        {
            ApplyFilter();
        }

        partial void OnSelectedPaintingChanged(Painting? oldValue, Painting newValue)
        {
            if(SelectedPainting != null && SelectedPainting.Id != 0)
            {
                ErrorHandler.SafeExecute(() =>
                {
                    Attachments = new ObservableCollection<Attachment>(attRep.GetListByArtPieceId(SelectedPainting.Id));
                    if(Attachments.Count > 0)
                    {
                        SelectedAttachment = Attachments[0];
                        SelectedImage = AttachmentHelper.LoadImageSource(SelectedAttachment.File);
                    }
                    else
                    {
                        SelectedAttachment = null;
                        SelectedImage = null;
                    }

                    Authors = new ObservableCollection<Artist>(artistRep.GetListByArtPieceId(SelectedPainting.Id));

                    var assignedIds = new HashSet<int>(artistRep.GetListByArtPieceId(SelectedPainting.Id).Select(a => a.Id));

                    var coll = artistRep.GetList().Where(a => !assignedIds.Contains(a.Id)).ToList();
                    AvailableArtists = new ObservableCollection<Artist>(coll);
                }, "Načtení detailů obrazu selhalo");
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
        private void AddArtistToPainting()
        {
            if (SelectedPainting == null || SelectedPainting.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte obraz");
                return;
            }

            if (SelectedArtistToAdd == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte umělce k přidání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                artistRep.AddArtistToArtPiece(SelectedArtistToAdd.Id, SelectedPainting.Id);
                Authors.Add(SelectedArtistToAdd);
                AvailableArtists.Remove(SelectedArtistToAdd);

                SelectedArtistToAdd = AvailableArtists.FirstOrDefault();
            }, "Přidání autora selhalo");
        }

        [RelayCommand]
        private void RemoveArtistFromPainting()
        {
            if (SelectedPainting == null || SelectedPainting.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte obraz");
                return;
            }

            if (SelectedArtistToRemove == null)
            {
                ErrorHandler.ShowError("Chyba", "Vyberte autora k odebrání");
                return;
            }

            ErrorHandler.SafeExecute(() =>
            {
                artistRep.RemoveArtistFromArtPiece(SelectedArtistToRemove.Id, SelectedPainting.Id);
                AvailableArtists.Add(SelectedArtistToRemove);
                Authors.Remove(SelectedArtistToRemove);

                SelectedArtistToRemove = Authors.FirstOrDefault();
            }, "Odebrání autora selhalo");
        }

        [RelayCommand]
        private void PreviousImage()
        {
            if(SelectedAttachment != null)
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
            if(SelectedAttachment != null)
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
            if (SelectedPainting == null || SelectedPainting.Id == 0)
            {
                ErrorHandler.ShowError("Chyba", "Nejprve uložte obraz");
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

                    attRep.SaveItem(att, SelectedPainting.Id);
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

        public PaintingViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            ErrorHandler.SafeExecute(() =>
            {
                _allPaintings = repository.GetList();
                Paintings = new ObservableCollection<Painting>(repository.GetList());
                Techniques = new ObservableCollection<Counter>(counterRep.GetTechniques());
                Bases = new ObservableCollection<Counter>(counterRep.GetFoundations());
                ApplyFilter();
            }, "Načtení obrazů selhalo");
        }

        private void ApplyFilter()
        {
            if (Paintings == null)
                return;

            var text = (SearchText ?? "").Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                Paintings = new ObservableCollection<Painting>(_allPaintings);
                return;
            }

            var lower = text.ToLowerInvariant();

            var filtered = Paintings
                .Where(p =>
                    (!string.IsNullOrWhiteSpace(p.Name) && p.Name.ToLowerInvariant().Contains(lower))
                )
                .ToList();

            Paintings = new ObservableCollection<Painting>(filtered);
        }

        [RelayCommand]
        private void New()
        {
            SelectedAttachment = null;
            SelectedImage = null;
            SelectedPainting = new Painting()
            {
                Base = new Counter(),
                Technique = new Counter()
            };
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedPainting == null)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(SelectedPainting.Name))
                {
                    ErrorHandler.ShowError("Validační chyba", "Název obrazu nesmí být prázdný");
                    return;
                }

                if (SelectedPainting.Height <= 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Výška musí být větší než 0");
                    return;
                }

                if (SelectedPainting.Width <= 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Šířka musí být větší než 0");
                    return;
                }

                if (SelectedPainting.Base == null || SelectedPainting.Base.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat podklad");
                    return;
                }

                if (SelectedPainting.Technique == null || SelectedPainting.Technique.Id == 0)
                {
                    ErrorHandler.ShowError("Validační chyba", "Musíte vybrat techniku");
                    return;
                }

                if (Bases != null)
                {
                    SelectedPainting.Base = Bases.FirstOrDefault(p => p.Id == SelectedPainting.Base.Id);
                }

                if (Techniques != null)
                {
                    SelectedPainting.Technique = Techniques.FirstOrDefault(p => p.Id == SelectedPainting.Technique.Id);
                }

                repository.SaveItem(SelectedPainting);
                Load();
            }, "Uložení obrazu selhalo.");
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedPainting == null || SelectedPainting.Id == 0)
                return;

            ErrorHandler.SafeExecute(() =>
            {
                repository.DeleteItem(SelectedPainting.Id);
                Load();
            }, "Smazání obrazu selhalo.");
        }
    }
}
