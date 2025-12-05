using Entities;
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

        void SaveItem(Sculpture sculpture);

        void DeleteItem(int id);
    }
}
