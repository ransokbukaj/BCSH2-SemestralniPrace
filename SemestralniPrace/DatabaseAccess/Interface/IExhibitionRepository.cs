using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IExhibitionRepository
    {
        List<Exhibition> GetList();

        void SaveItem(Exhibition exhibition);

        void DeleteItem(int id);
    }
}
