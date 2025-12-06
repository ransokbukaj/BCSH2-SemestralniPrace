using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace DatabaseAccess
{
    public class AddressRepository : IAddressRepository
    {
        public List<Address> GetList()
        {
            throw new NotImplementedException();
        }

        public void SaveItem(Address address)
        {
            using (var connection = ConnectionManager.Connection)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "p_save_adresa";

                    var paramId = new OracleParameter
                    {
                        ParameterName = "p_idadresa",
                        OracleDbType = OracleDbType.Int32,
                        Direction = System.Data.ParameterDirection.InputOutput,
                        Value = address.Id == 0 ? (object)DBNull.Value : address.Id
                    };
                    command.Parameters.Add(paramId);

                    var paramUlice = new OracleParameter
                    {
                        ParameterName = "p_ulice",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = address.Street
                    };
                    command.Parameters.Add(paramUlice);

                    var paramCisloPopisne = new OracleParameter
                    {
                        ParameterName = "p_cislopopisne",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = address.HouseNumber
                    };
                    command.Parameters.Add(paramCisloPopisne);

                    var paramCisloOrientacni = new OracleParameter
                    {
                        ParameterName = "p_cisloorientacni",
                        OracleDbType = OracleDbType.Varchar2,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = string.IsNullOrEmpty(address.StreetNumber) ? (object)DBNull.Value : address.StreetNumber
                    };
                    command.Parameters.Add(paramCisloOrientacni);

                    var paramIdPosta = new OracleParameter
                    {
                        ParameterName = "p_idposta",
                        OracleDbType = OracleDbType.Int32,
                        Direction = System.Data.ParameterDirection.Input,
                        Value = address.Post.Id
                    };
                    command.Parameters.Add(paramIdPosta);

                    // Provedení procedury
                    command.ExecuteNonQuery();

                    // Commit transakce
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        public void DeleteItem(int id)
        {
            throw new NotImplementedException();
        }
    }
}
