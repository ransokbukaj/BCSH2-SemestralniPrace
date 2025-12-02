using Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IEducationProgramRepository
    {
        List<EducationProgram> GetList();

        void SaveItem(EducationProgram educationProgram);

        void DeleteItem(int id);
    }
}
