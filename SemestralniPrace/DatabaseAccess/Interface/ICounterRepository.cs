using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface ICounterRepository
    {
        List<Counter> GetFoundations();
        List<Counter> GetMaterials();
        List<Counter> GetPaymentMethods();
        List<Counter> GetRoles();
        List<Counter> GetTechniques();
        List<VisitType> GetVisitTypes();
        List<Counter> GetExhibitionCounters();
        List<Counter> GetArtPieceCounters();
    }
}
