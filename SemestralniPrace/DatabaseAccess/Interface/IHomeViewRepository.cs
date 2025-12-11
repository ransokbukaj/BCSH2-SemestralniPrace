using Entities.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IHomeViewRepository
    {

        List<AvailableExhibition> GetAvailableExhibitions();
        List<NewArtPiece> GetNewArtPieces();

        GaleryStatistic GetGaleryStatistic();
    }
}
