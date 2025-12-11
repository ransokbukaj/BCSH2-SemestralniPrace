using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Home
{
    public class GaleryStatistic
    {
        public int ArtPieceAmount { get; set; } = 0;
        public int PaintingAmount { get; set; } = 0;
        public int SculptureAmount { get; set; } = 0;
        public int ExhibitonAmount { get; set; } = 0;
        public int EducationProgramAmount { get; set; } = 0;
        public int ArtistAmount { get; set; } = 0;
        public int VisitorAmount { get; set; } = 0;

        public double Sales { get; set; } = 0;

    }
}
