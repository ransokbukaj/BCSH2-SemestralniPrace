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
        //Aktualní přihlášený uživatel.
        public static User? CurrentUser;

        //Pomocná proměná pro emulaci
        public static bool isEmulating;
        //Pomocná proměná pro držení skutečně přihlášeného uživatele během emulace.
        private static User loggedUser;

        static UserManager()
        {
            CurrentUser = null;
        }

        /// <summary>
        /// Metoda pro registraci nového uživatele.
        /// </summary>
        /// <param name="username">Uživatelské jméno uživatele</param>
        /// <param name="password">Heslo v textové podobě</param>
        /// <param name="firstName">Křestní jméno uživatele</param>
        /// <param name="lastName">Příjmení uživatele</param>
        /// <param name="email">Email uživatele</param>
        /// <param name="phoneNumber">Telefon uživatele</param>
        /// <returns>True/False podle toho jestli se uživatel přihlásil.</returns>
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

        /// <summary>
        /// Metoda pro přihlášení uživatele. Při úspěšném přihlášení se automaticky nastaví CurrentUser na uživatele.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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
                            CurrentUser = new User
                            {
                                Id = userId,
                                Username = reader["uzivatelskejmeno"].ToString(),
                                FirstName = reader["jmeno"].ToString(),
                                LastName = reader["prijmeni"].ToString(),
                                Role = new Counter
                                {
                                    Id = Convert.ToInt32(reader["idrole"]),
                                    Name = reader["nazevrole"].ToString()
                                },
                                RegisterDate = Convert.ToDateTime(reader["datumregistrace"]),
                                LastLogin = DateTime.Now,
                            };
                            UpdateLastLoginDate(ConnectionManager.Connection, userId);
                            //Vytvoření Session s id uživatele v databázi
                            SetDatabaseSessionIdentifier(ConnectionManager.Connection, userId);
                            return true;
                        }
                    }
                }
            }
            return false;            
        }

        /// <summary>
        /// Metoda pro odhlášení uživatele. Metoda nastaví hodnotu CurrentUser na NULL.
        /// </summary>
        public static void LogOut()
        {
            ClearDatabaseSessionIdentifier(ConnectionManager.Connection);
            CurrentUser = null;
        }

        /// <summary>
        /// Pomocná metoda sloužící k hashování hesla.
        /// </summary>
        /// <param name="password">Heslo v textové podobě</param>
        /// <returns></returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        /// <summary>
        /// Pomocná metoda pro ověření hesla
        /// </summary>
        /// <param name="password">Zadané heslo v textové podobě</param>
        /// <param name="storedHash">Uložený Hash</param>
        /// <returns>True/False podle toho jestli se heslo schoduje.</returns>
        private static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash))
            {
                return false;
            }
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        /// <summary>
        /// Metoda pro start emulace jiného uživatele.
        /// </summary>
        /// <param name="target">Cílený uživatel k emulaci.</param>
        /// <exception cref="Exception"></exception>
        public static void StartEmulatingUser(User target)
        {
            if(CurrentUser == null)
            {
                throw new Exception("Není možno simulatovat bez přihlášení.");
            }

            //Emulovat může pouze administrátor.
            if(CurrentUser.Role.Name != "Admin")
            {
                throw new Exception("Simulovat může jen uživatel s oprávněním administrátora.");
            }

            if (isEmulating)
            {
                throw new Exception("Není možno simulatovat jiného uživatele, když už se simuluje.");
            }

            isEmulating = true;
            loggedUser = CurrentUser;
            LogOut();

            CurrentUser = target;
            SetDatabaseSessionIdentifier(ConnectionManager.Connection, CurrentUser.Id);
        }

        /// <summary>
        /// Metoda pro ukončení emulace jiného uživatele
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void EndEmulatingUser()
        {
            if (!isEmulating)
            {
                throw new Exception("Není možné ukončit neexistujicí simulaci.");
            }
            LogOut();
            CurrentUser = loggedUser;
            SetDatabaseSessionIdentifier(ConnectionManager.Connection, CurrentUser.Id);
            isEmulating= false;
        }

        /// <summary>
        /// Pomocná metoda pro vytvoření Session s id uživatele.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userId"></param>
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

        /// <summary>
        /// Pomocná metoda pro vyčistění Session
        /// </summary>
        /// <param name="connection">Connection k používané databázi.</param>
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

        /// <summary>
        /// Pomocná metoda pro nastavění změny posledního přihlášení.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="userId">Id uživatele.</param>
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