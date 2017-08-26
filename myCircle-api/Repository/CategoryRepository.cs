using System.Collections.Generic;
using System.Data;
using Dapper;
using myCircle_api.Model;
using MySql.Data.MySqlClient;

namespace myCircle_api.Repository
{
    public class CategoryRepository
    {
        private readonly string connectionString;
        public CategoryRepository()
        {
            connectionString = Startup.Configuration.GetSection("ConnectionStrings:Main").Value;
        }
 
        public IDbConnection Connection => new MySqlConnection(connectionString);

        public void Add(Category category)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "INSERT INTO category (name)"
                                + " VALUES(@name)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, category);
            }
        }
 
        public IEnumerable<Category> GetAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Category>("SELECT * FROM category");
            }
        }
 
        public void Delete(string name)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "DELETE FROM category"
                                + " WHERE name = @name";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { name });
            }
        }
 
        public void Update(Category category)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "UPDATE category SET name = @name"
                                + " WHERE name = @name";
                dbConnection.Open();
                dbConnection.Query(sQuery, category);
            }
        }
    }
}