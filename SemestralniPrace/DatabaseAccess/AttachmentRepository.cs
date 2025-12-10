using DatabaseAccess.Interface;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class AttachmentRepository : IAttachmentRepository
    {
        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

        public List<Attachment> GetList()
        {
            throw new NotImplementedException();
        }

        public List<Attachment> GetListByArtPieceId(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveItem(Attachment attachment)
        {
            throw new NotImplementedException();
        }
    }
}
