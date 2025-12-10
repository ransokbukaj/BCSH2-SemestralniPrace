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

        [ObservableProperty]
        private ObservableCollection<Sculpture> sculptures = new();

        [ObservableProperty]
        private ObservableCollection<Counter> materials = new();

        [ObservableProperty]
        private ObservableCollection<Attachment> attachments = new();


        [ObservableProperty]
        private Sculpture selectedSculpture;

        [ObservableProperty]
        private Attachment selectedAttachment;

        [ObservableProperty]
        private ImageSource selectedImage;


        partial void OnSelectedSculptureChanged(Sculpture? oldValue, Sculpture newValue)
        {
            if (SelectedSculpture != null && SelectedSculpture.Id != 0)
            {
                Attachments = new ObservableCollection<Attachment>(attRep.GetListByArtPieceId(SelectedSculpture.Id));
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
            }
        }

        partial void OnSelectedAttachmentChanged(Attachment value)
        {
            if (SelectedAttachment != null)
                SelectedImage = AttachmentHelper.LoadImageSource(SelectedAttachment.File);
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

                Attachments.Add(att);
                SelectedAttachment = att;
                attRep.SaveItem(att, SelectedSculpture.Id);

            }

        }

        [RelayCommand]
        private void RemoveCurrentImage()
        {
            if (SelectedAttachment != null)
            {
                SelectedImage = null;
                Attachments.Remove(SelectedAttachment);
                attRep.DeleteItem(SelectedAttachment.Id);
                SelectedAttachment = Attachments.FirstOrDefault();
            }

        }




        public SculptureViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Sculptures = new ObservableCollection<Sculpture>(repository.GetList());
            Materials = new ObservableCollection<Counter>(counterRep.GetMaterials());
        }

        [RelayCommand]
        private void New()
        {
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


            if(Materials != null && SelectedSculpture != null)
            {
                SelectedSculpture.Material = Materials.FirstOrDefault(p => p.Id == SelectedSculpture.Material.Id);
            }

            repository.SaveItem(SelectedSculpture);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedSculpture == null || SelectedSculpture.Id == 0)
                return;

            repository.DeleteItem(SelectedSculpture.Id);
            Load();
        }
    }
}
