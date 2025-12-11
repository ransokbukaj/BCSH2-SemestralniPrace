using Entities;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess
{
    public static class UserManager
    {
        public static User? CurrentUser;

        public static bool isSimulating;
        private static User loggedUser;

        static UserManager()
        {
            CurrentUser = null;
        }

        public static bool Register(string username, string password, string firstName, string lastName, string email, string phoneNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                {
                    return false;
                }

                // Kontrola, zda uživatel již neexistuje
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM uzivatele 
                    WHERE uzivatelskejmeno = :username";

                using (var command = ConnectionManager.Connection.CreateCommand())
                {
                    var param = command.CreateParameter();
                    param.ParameterName = ":username";
                    param.Value = username;

                    command.CommandText = checkQuery;
                    command.Parameters.Add(param);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count > 0)
                    {
                        // Uživatelské jméno již existuje
                        return false;
                    }
                }

                var newUser = new User
                {
                    Id = 0, // Nový uživatel
                    Username = username,
                    Password = password, // Bude zahashováno v SaveItem
                    FirstName = firstName,
                    LastName = lastName,
                    Email = string.IsNullOrWhiteSpace(email) ? null : email,
                    PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber,
                    Role = new Counter
                    {
                        Id = 2 // Výchozí role pro nové uživatele
                    }
                };

                var repository = new UserRepository();
                repository.SaveItem(newUser);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool LogIn(string username, string password)
        {
            string query = @"
                SELECT
                    u.iduzivatel, 
                    u.uzivatelskejmeno, 
                    u.heslohash, 
                    u.jmeno, 
                    u.prijmeni, 
                    u.idrole,
                    u.deaktivovan,
                    u.datumregistrace,
                    u.datumposlednihoprihlaseni,
                    r.nazev as nazevrole
                FROM
                    uzivatele u
                    INNER JOIN role r
                    ON u.idrole = r.idrole
                WHERE
                    u.uzivatelskejmeno = :username";

            using (var command = ConnectionManager.Connection.CreateCommand())
            {
                var param = command.CreateParameter();
                param.ParameterName = ":username";
                param.Value = username;

                command.CommandText = query;
                command.Parameters.Add(param);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int deaktivovan = Convert.ToInt32(reader["deaktivovan"]);
                        if (deaktivovan == 1)
                        {
                            return false;
                        }

                        string storedHash = reader["heslohash"].ToString();

                        if (VerifyPassword(password, storedHash))
                        {
                            int userId = Convert.ToInt32(reader["iduzivatel"]);
                            int roleId = Convert.ToInt32(reader["idrole"]);

                            CurrentUser = new User
                            {
                                Id = userId,
                                Username = reader["uzivatelskejmeno"].ToString(),
                                FirstName = reader["jmeno"].ToString(),
                                LastName = reader["prijmeni"].ToString(),
                                Role = new Counter
                                {
                                    Id = roleId,
                                    Name = reader["nazevrole"].ToString()
                                },
                                RegisterDate = Convert.ToDateTime(reader["datumregistrace"]),
                                LastLogin = reader["datumposlednihoprihlaseni"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["datumposlednihoprihlaseni"]),
                            };

                            SetDatabaseSessionIdentifier(ConnectionManager.Connection, userId);
                            UpdateLastLoginDate(ConnectionManager.Connection, userId);
                            return true;
                        }
                    }
                }
            }
            return false;            
        }

        public static void LogOut()
        {
            ClearDatabaseSessionIdentifier(ConnectionManager.Connection);
            CurrentUser = null;
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        private static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        public static void StartSimulateUser(User target)
        {
            if(CurrentUser == null)
            {
                throw new Exception("Není možno simulatovat bez přihlášení.");
            }
            if(CurrentUser.Role.Name != "Admin")
            {
                throw new Exception("Simulovat může jen uživatel s oprávněním administrátora.");
            }

            if (isSimulating)
            {
                throw new Exception("Není možno simulatovat jiného uživatele, když už se simuluje.");
            }

            isSimulating = true;
            loggedUser = CurrentUser;
            LogOut();

            CurrentUser = target;
            SetDatabaseSessionIdentifier(ConnectionManager.Connection, CurrentUser.Id);
        }

        public static void EndSimulatingUser()
        {
            if (!isSimulating)
            {
                throw new Exception("Není možné ukončit neexistujicí simulaci.");
            }
            LogOut();
            CurrentUser = loggedUser;
            SetDatabaseSessionIdentifier(ConnectionManager.Connection, CurrentUser.Id);
            isSimulating= false;
        }

        private static void SetDatabaseSessionIdentifier(System.Data.IDbConnection connection, int userId)
        {
            string query = @"
                BEGIN
                    DBMS_SESSION.SET_IDENTIFIER(:userId);
                END;";

            using (var command = connection.CreateCommand())
            {
                var param = command.CreateParameter();
                param.ParameterName = ":userId";
                param.Value = userId.ToString();

                command.CommandText = query;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }

        private static void ClearDatabaseSessionIdentifier(System.Data.IDbConnection connection)
        {
            string query = @"
                BEGIN
                    DBMS_SESSION.CLEAR_IDENTIFIER();
                END;";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        private static void UpdateLastLoginDate(System.Data.IDbConnection connection, int userId)
        {
            string query = @"
                UPDATE uzivatele
                SET datumposlednihoprihlaseni = SYSDATE
                WHERE iduzivatel = :userId";

            using (var command = connection.CreateCommand())
            {
                var param = command.CreateParameter();
                param.ParameterName = ":userId";
                param.Value = userId;

                command.CommandText = query;
                command.Parameters.Add(param);
                command.ExecuteNonQuery();
            }
        }
    }
}