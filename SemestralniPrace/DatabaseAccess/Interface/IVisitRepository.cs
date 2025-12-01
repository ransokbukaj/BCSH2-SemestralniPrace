using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IVisitRepository
    {
        List<Visit> GetList();

        bool SaveItem(Visit item);

        bool DeleteItem(int itemId);
    }
}
