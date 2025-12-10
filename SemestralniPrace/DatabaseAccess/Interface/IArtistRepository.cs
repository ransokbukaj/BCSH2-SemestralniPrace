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

        void SaveItem(Artist artist);

        void DeleteItem(int id);
    }
}
