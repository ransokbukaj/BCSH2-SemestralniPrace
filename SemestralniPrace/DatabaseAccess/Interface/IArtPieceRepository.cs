using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IArtPieceRepository
    {
        List<ArtPiece> GetList();

        List<ArtPiece> GetListByArtistId(int artistId);

        List<ArtPiece> GetListByExhibitionId(int exhibitionId);

        List<ArtPiece> GetListInStorage();

        List<ArtPiece> GetListBySaleId(int saleId);

        void SaveItem(ArtPiece piece);

        void DeleteItem(int id);

        void AddArtPieceToExhibition(int idArtpiece, int idExhibition);

        void RemoveArtPieceFromExhibition(int idArtpiece, int idExhibition);
    }
}
