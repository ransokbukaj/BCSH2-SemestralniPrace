using Entities.Data;
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

        bool SaveItem(Post item);

        bool DeleteItem(int itemId);
    }
}
