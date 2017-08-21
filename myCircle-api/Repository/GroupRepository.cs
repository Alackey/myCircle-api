using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using myCircle_api.Model;
using MySql.Data.MySqlClient;

namespace myCircle_api.Repository
{
    public class GroupRepository
    {
        private readonly string connectionString;
        public GroupRepository()
        {
            connectionString = Startup.Configuration.GetSection("ConnectionStrings:Main").Value;
        }
 
        public IDbConnection Connection => new MySqlConnection(connectionString);

        public Group Add(Group group)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "INSERT INTO groups (id, name, privateVis, groupPage, photoUrl, backgroundPhotoUrl, " + 
                                 "description, notificationsId, eventsId, category, type, officialClub, discoverable) " + 
                                "VALUES(@id, @name, @privateVis, @groupPage, @photoUrl, @backgroundPhotoUrl, " +
                                 "@description, @notificationsId, @eventsId, @category, @type, @officialClub, @discoverable)";
                dbConnection.Open();
                
                group.Id = Guid.NewGuid().ToString();
                dbConnection.Execute(sQuery, group);
                
                return group;
            }
        }
 
        public IEnumerable<Group> GetAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Group>("SELECT * FROM groups");
            }
        }
 
        public Group GetById(string id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "SELECT * FROM groups"
                                + " WHERE id = @id";
                dbConnection.Open();
                return dbConnection.Query<Group>(sQuery, new { id }).FirstOrDefault();
            }
        }
 
        public void Delete(string id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "DELETE FROM groups"
                                + " WHERE id = @id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { id });
            }
        }
 
        public Group Update(string id, Group group)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "UPDATE groups SET name = @name, privateVis = @privateVis, groupPage = @groupPage, " +
                                 "photoUrl = @photoUrl, backgroundPhotoUrl = @backgroundPhotoUrl, description = @description, " +
                                 "notificationsId = @notificationsId, eventsId = @eventsId, category = @category, " +
                                 "type = @type, officialClub = @officialClub, discoverable = @discoverable " + 
                                "WHERE id = @id";
                dbConnection.Open();
                dbConnection.Query(sQuery, group);
                return group;
            }
        }
    }
}