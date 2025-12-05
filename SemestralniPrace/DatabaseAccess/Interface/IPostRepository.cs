using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IPostRepository
    {
        List<Post> GetList();

        void SaveItem(Post post);

        void DeleteItem(int id);
    }
}
