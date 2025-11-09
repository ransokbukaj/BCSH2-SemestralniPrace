using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabaseAccess.Interface
{
    public interface ILoadData
    {
        List<Adress> LoadAdresses();
        List<Artist> LoadArtists();
        List<ArtPiece> LoadArtPieces();
        List<Buyer> LoadBuyers();
        List<EducationProgram> LoadEducationPrograms();
        List<Exhibition> LoadExhibitions();
        List<Foundation> LoadFoundations();
        List<Material> LoadMaterials();
        List<Painting> LoadPaintings();
        List<PaymentMethod> LoadPaymentMethods();
        List<Post> LoadPosts();
        List<Sale> LoadSales();
        List<Sculpture> LoadSculptures();
        List<Technique> LoadTechniques();
        List<Visit> LoadVisits();
        List<VisitType> LoadVisitTypes();


    }
}
