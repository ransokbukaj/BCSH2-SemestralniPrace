using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class CounterRepository : ICounterRepository
    {
        public List<Counter> GetFoundations()
        {
            throw new NotImplementedException();
        }

        public List<Counter> GetMaterials()
        {
            throw new NotImplementedException();
        }

        public List<Counter> GetPaymentMethods()
        {
            throw new NotImplementedException();
        }

        public List<Counter> GetRoles()
        {
            throw new NotImplementedException();
        }

        public List<Counter> GetTechniques()
        {
            throw new NotImplementedException();
        }

        public List<VisitType> GetVisitTypes()
        {
            throw new NotImplementedException();
        }
    }
}
