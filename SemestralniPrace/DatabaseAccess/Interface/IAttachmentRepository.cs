using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess.Interface
{
    internal interface IAttachmentRepository
    {
        List<Attachment> GetList();

        List<Attachment> GetListByArtPieceId(int id);

        void SaveItem(Attachment attachment);

        void DeleteItem(int id);
    }
}
