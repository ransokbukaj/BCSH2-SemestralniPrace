using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public class AttachmentRepository : IAttachmentRepository
    {
        public List<Attachment> GetList()
        {
            throw new NotImplementedException();
        }

        public List<Attachment> GetListByArtPieceId(int id)
        {
            var list = new List<Attachment>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        idpriloha,
                        soubor,
                        typsouboru,
                        nazevsouboru,
                   
                    FROM prilohy
                    WHERE idumeleckedilo = :umeleckeId
                    ORDER BY nazevsouboru";

                var paramId = new OracleParameter
                {
                    ParameterName = "umeleckeId",
                    OracleDbType = OracleDbType.Int32,
                    Value = id
                };
                command.Parameters.Add(paramId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idTemp = Convert.ToInt32(reader["idpriloha"]);
                        long blobLength = reader.GetBytes( 0, 0,  null,  0, 0);
                        byte[] fileTemp = new byte[blobLength];
                        reader.GetBytes(0, 0, fileTemp, 0, fileTemp.Length);

                        list.Add(new Attachment
                        {
                            Id = idTemp,
                            File = fileTemp,
                            FileType = reader["typsouboru"].ToString(),
                            FileName = reader["nazevsouboru"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(Attachment attachment)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }

    }
}
