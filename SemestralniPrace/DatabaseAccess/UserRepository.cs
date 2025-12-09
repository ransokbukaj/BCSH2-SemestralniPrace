using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseAccess.Interface;
using Entities;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseAccess
{
    public class UserRepository : IUserRepository
    {
        public List<User> GetList()
        {
            var list = new List<User>();
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandText = @"
                    SELECT 
                        id,
                        uzivatelske_jmeno,
                        jmeno,
                        prijmeni,
                        email,
                        telefonni_cislo,
                        datum_registrace,
                        datum_posledniho_prihlaseni,
                        datum_posledni_zmeny,
                        id_role,
                        nazev_role
                    FROM v_uzivatele";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Username = reader["uzivatelske_jmeno"].ToString(),
                            FirstName = reader["jmeno"].ToString(),
                            LastName = reader["prijmeni"].ToString(),
                            Email = reader["email"] == DBNull.Value ? null : reader["email"].ToString(),
                            PhoneNumber = reader["telefonni_cislo"] == DBNull.Value ? null : reader["telefonni_cislo"].ToString(),
                            RegisterDate = Convert.ToDateTime(reader["datum_registrace"]),
                            LastLogin = reader["datum_posledniho_prihlaseni"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["datum_posledniho_prihlaseni"]),
                            LastChange = reader["datum_posledni_zmeny"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["datum_posledni_zmeny"]),
                            Role = new Counter
                            {
                                Id = Convert.ToInt32(reader["id_role"]),
                                Name = reader["nazev_role"].ToString()
                            }
                        });
                    }
                }
            }
            return list;
        }

        public void SaveItem(User user)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_save_uzivatel";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_iduzivatel",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = user.Id == 0 ? (object)DBNull.Value : user.Id
                };
                command.Parameters.Add(paramId);

                var paramUsername = new OracleParameter
                {
                    ParameterName = "p_uzivatelskejmeno",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = user.Username
                };
                command.Parameters.Add(paramUsername);

                // Pro nového uživatele použijeme Password (plain text, který se zahashuje)
                // Pro existujícího uživatele, pokud není Password prázdný, zahashujeme ho
                string passwordHash = null;
                if (!string.IsNullOrEmpty(user.Password))
                {
                    passwordHash = UserManager.HashPassword(user.Password);
                }

                var paramPassword = new OracleParameter
                {
                    ParameterName = "p_heslohash",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = string.IsNullOrEmpty(passwordHash) ? (object)DBNull.Value : passwordHash
                };
                command.Parameters.Add(paramPassword);

                var paramJmeno = new OracleParameter
                {
                    ParameterName = "p_jmeno",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = user.FirstName
                };
                command.Parameters.Add(paramJmeno);

                var paramPrijmeni = new OracleParameter
                {
                    ParameterName = "p_prijmeni",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = user.LastName
                };
                command.Parameters.Add(paramPrijmeni);

                var paramEmail = new OracleParameter
                {
                    ParameterName = "p_email",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = string.IsNullOrEmpty(user.Email) ? (object)DBNull.Value : user.Email
                };
                command.Parameters.Add(paramEmail);

                var paramTelefon = new OracleParameter
                {
                    ParameterName = "p_telefonicislo",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = string.IsNullOrEmpty(user.PhoneNumber) ? (object)DBNull.Value : user.PhoneNumber
                };
                command.Parameters.Add(paramTelefon);

                var paramRole = new OracleParameter
                {
                    ParameterName = "p_idrole",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = user.Role.Id
                };
                command.Parameters.Add(paramRole);

                // Provedení procedury
                command.ExecuteNonQuery();

                // Commit transakce
                using (var transaction = ConnectionManager.Connection.BeginTransaction())
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

        public void DeleteItem(int id)
        {
            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_delete_uzivatel";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_iduzivatel",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = id
                };
                command.Parameters.Add(paramId);

                // Provedení procedury
                command.ExecuteNonQuery();

                // Commit transakce
                using (var transaction = ConnectionManager.Connection.BeginTransaction())
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

        public void ChangePassword(int id, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("Nové heslo nesmí být prázdné.", nameof(newPassword));
            }

            string passwordHash = UserManager.HashPassword(newPassword);

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "p_change_password";

                var paramId = new OracleParameter
                {
                    ParameterName = "p_iduzivatel",
                    OracleDbType = OracleDbType.Int32,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = id
                };
                command.Parameters.Add(paramId);

                var paramPassword = new OracleParameter
                {
                    ParameterName = "p_noveheslohash",
                    OracleDbType = OracleDbType.Varchar2,
                    Direction = System.Data.ParameterDirection.Input,
                    Value = passwordHash
                };
                command.Parameters.Add(paramPassword);

                // Provedení procedury
                command.ExecuteNonQuery();

                // Commit transakce
                using (var transaction = ConnectionManager.Connection.BeginTransaction())
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
}
