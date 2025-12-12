using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IArtistRepository
    {
        List<Artist> GetList();

        List<Artist> GetListByArtPieceId(int id);
        List<Artist> GetAvailableMentors(int idOfArtist);
        void SaveItem(Artist artist);

        void DeleteItem(int id);


        void AddArtistToArtPiece(int id, int idArt);
        void RemoveArtistFromArtPiece(int id, int idArt);
    }
}
