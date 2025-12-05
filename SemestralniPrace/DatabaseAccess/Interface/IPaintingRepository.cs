using Entities;
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

        void SaveItem(Painting painting);

        void DeleteItem(int id);
    }
}
