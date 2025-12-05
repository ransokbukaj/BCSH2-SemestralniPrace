using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;

namespace DatabaseAccess
{
    public class PostRepository : IPostRepository
    {
        public List<Post> GetList()
        {
            throw new NotImplementedException();
        }

        public void SaveItem(Post post)
        {
            if (post.Id == 0)
            {
                // insert
            }
            else
            {
                // update
            }
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
