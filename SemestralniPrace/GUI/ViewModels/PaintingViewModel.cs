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
using System.Windows.Media;

namespace GUI.ViewModels
{
    public partial class PaintingViewModel : ObservableObject
    {
        private readonly PaintingRepository repository = new PaintingRepository();
        private readonly CounterRepository counterRep = new CounterRepository();
        private readonly AttachmentRepository attRep = new AttachmentRepository();



        [ObservableProperty]
        private ObservableCollection<Painting> paintings = new();

        [ObservableProperty]
        private ObservableCollection<Counter> techniques = new();

        [ObservableProperty]
        private ObservableCollection<Counter> bases = new();

        [ObservableProperty]
        private ObservableCollection<Attachment> attachments = new();

        
        [ObservableProperty]
        private Painting selectedPainting;




        [ObservableProperty]
        private Attachment selectedAttachment;

        [ObservableProperty]
        private ImageSource selectedImage;


        partial void OnSelectedPaintingChanged(Painting? oldValue, Painting newValue)
        {
            if(SelectedPainting != null && SelectedPainting.Id != 0)
            {
                Attachments = new ObservableCollection<Attachment>(attRep.GetListByArtPieceId(SelectedPainting.Id));
                if(Attachments.Count > 0)
                {
                    SelectedAttachment = Attachments[0];
                    SelectedImage = AttachmentHelper.LoadImageSource(SelectedAttachment.File);
                }
            }
        }

        partial void OnSelectedAttachmentChanged(Attachment value)
        {
            SelectedImage = AttachmentHelper.LoadImageSource(SelectedAttachment.File);
        }

        [RelayCommand]
        private void PreviousImage()
        {
            int cur = Attachments.IndexOf(SelectedAttachment);
            if(cur == 0)
            {
                SelectedAttachment = Attachments.Last();
            }
            else
            {
                SelectedAttachment = Attachments[cur - 1];
            }
        }

        [RelayCommand]
        private void NextImage()
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

        [RelayCommand]
        private void AddImage()
        {

        }

        [RelayCommand]
        private void RemoveCurrentImage()
        {




        }




        public PaintingViewModel()
        {
            Load();
        }

        [RelayCommand]
        private void Load()
        {
            Paintings = new ObservableCollection<Painting>(repository.GetList());
            Techniques = new ObservableCollection<Counter>(counterRep.GetTechniques());
            Bases = new ObservableCollection<Counter>(counterRep.GetFoundations());
            
        }

        [RelayCommand]
        private void New()
        {
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


            if (Bases != null)
            {
                // sjednotit instanci Post podle Id
                SelectedPainting.Base = Bases.FirstOrDefault(p => p.Id == SelectedPainting.Base.Id);
            }

            if (Techniques != null)
            {
                // sjednotit instanci Post podle Id
                SelectedPainting.Technique = Techniques.FirstOrDefault(p => p.Id == SelectedPainting.Technique.Id);
            }

            repository.SaveItem(SelectedPainting);
            Load();
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedPainting == null || SelectedPainting.Id == 0)
                return;

            repository.DeleteItem(SelectedPainting.Id);
            Load();
        }
    }
}
