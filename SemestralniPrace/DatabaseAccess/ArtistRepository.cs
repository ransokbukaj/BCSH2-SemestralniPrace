using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class ArtistRepository : IArtistRepository
    {
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Artist> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Artist item)
        {
            throw new NotImplementedException();
        }
    }
}
