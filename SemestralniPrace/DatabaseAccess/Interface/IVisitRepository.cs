using Entities;
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

        void SaveItem(Visit visit, int idExhibit);

        void DeleteItem(int id);
    }
}
