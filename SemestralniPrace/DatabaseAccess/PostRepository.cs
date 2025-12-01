using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities.Data;

namespace DatabaseAccess
{
    public class PostRepository : IPostRepository
    {
        public bool DeleteItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public List<Post> GetList()
        {
            throw new NotImplementedException();
        }

        public bool SaveItem(Post item)
        {
            throw new NotImplementedException();
        }
    }
}
