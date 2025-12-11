using Entities;
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

        List<Exhibition> GetListByProgramId(int  programId);

        void SaveItem(Exhibition exhibition);

        void DeleteItem(int id);

        void AddExhibitionToProgram(int idEx, int idProgram);

        void RemoveExhibitionFromProgram(int idEx);
        
    }
}
