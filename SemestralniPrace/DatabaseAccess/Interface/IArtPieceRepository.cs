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

        void SaveItem(ArtPiece piece);

        void DeleteItem(int id);
    }
}
