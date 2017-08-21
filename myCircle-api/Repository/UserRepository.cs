using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using myCircle_api.Model;
using MySql.Data.MySqlClient;

namespace myCircle_api.Repository
{
    public class UserRepository
    {
        private readonly string connectionString;
        public UserRepository()
        {
            connectionString = Startup.Configuration.GetSection("ConnectionStrings:Main").Value;
        }
 
        public IDbConnection Connection => new MySqlConnection(connectionString);

        public void Add(User user)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "INSERT INTO users (username, photoUrl, firstname, lastname, email, second_email)"
                                + " VALUES(@username, @photoUrl, @firstname, @lastname, @email, @second_email)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, user);
            }
        }
 
        public IEnumerable<User> GetAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<User>("SELECT * FROM users");
            }
        }
 
        public User GetByUsername(string username)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "SELECT * FROM users"
                                + " WHERE username = @username";
                dbConnection.Open();
                return dbConnection.Query<User>(sQuery, new { username }).FirstOrDefault();
            }
        }
 
        public void Delete(string username)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "DELETE FROM users"
                                + " WHERE username = @username";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { username });
            }
        }
 
        public void Update(User user)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "UPDATE users SET photoUrl = @photoUrl, firstname = @firstname, lastname = @lastname,"
                                + " email = @email, second_email = @second_email"
                                + " WHERE username = @username";
                dbConnection.Open();
                dbConnection.Query(sQuery, user);
            }
        }
    }
}