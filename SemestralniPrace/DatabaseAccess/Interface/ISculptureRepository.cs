using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface ISculptureRepository
    {
        List<Sculpture> GetList();

        bool SaveItem(Sculpture item);

        bool DeleteItem(int itemId);
    }
}
