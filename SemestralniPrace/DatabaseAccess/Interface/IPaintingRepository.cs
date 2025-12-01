using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IPaintingRepository
    {
        List<Painting> GetList();

        bool SaveItem(Painting item);

        bool DeleteItem(int itemId);
    }
}
