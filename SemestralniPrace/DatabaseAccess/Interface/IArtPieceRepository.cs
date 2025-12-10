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

        //Díla co nejsou na žádné výstavì.
        List<ArtPiece> GetListInStorage();

        List<ArtPiece> GetListBySaleId(int saleId);

        void SaveItem(ArtPiece piece);

        void DeleteItem(int id);
    }
}
