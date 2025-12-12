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

        List<ArtPiece> GetListByArtistId(int id);

        List<ArtPiece> GetListByExhibitionId(int id);

        List<ArtPiece> GetListInStorage();

        List<ArtPiece> GetListBySaleId(int id);

        List<ArtPiece> GetListUnsold();

        void AddArtPieceToExhibition(int idArt, int idExhibition);

        void RemoveArtPieceFromExhibition(int idArt, int idExhibition);
    }
}
