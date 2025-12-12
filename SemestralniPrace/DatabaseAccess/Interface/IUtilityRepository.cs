using Entities.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IUtilityRepository
    {

        List<AvailableExhibition> GetAvailableExhibitions();

        List<NewArtPiece> GetNewArtPieces();

        GaleryStatistics GetGaleryStatistic();

        ArtistStatistics GetArtistStatistic(int id);

        MentorBranchStatistics GetMentorBranchStatics(int id);

        MostSuccesfulMentore GetMostSuccesfulMentore(int idArtist);
        UserStatistics GetUserStatistics(int id);
    }
}
