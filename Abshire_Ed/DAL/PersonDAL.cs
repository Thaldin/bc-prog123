using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using Abshire_Ed.Models;

namespace Abshire_Ed.DAL
{
    public class PersonDAL
    {
        private readonly IConfiguration _configuration;

        const string _connStrKey = "MyConnString";

        public PersonDAL(IConfiguration config)
        {
            _configuration = config;
        }

        public PersonModel GetPerson(string id)
        {
            const string personSelect = "SELECT * FROM [dbo].[Person] WHERE PersonId = @pId";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(personSelect, conn);
                sqlCmd.Parameters.AddWithValue("@pId", id);
                
                var sqlReader = sqlCmd.ExecuteReader();
                sqlReader.Read();
                var person = new PersonModel()
                {
                    FirstName = sqlReader["FName"].ToString(),
                    LastName = sqlReader["LName"].ToString(),
                    Address = sqlReader["address"].ToString(),
                    Email = sqlReader["email"].ToString(),
                    Phone = sqlReader["phone"].ToString(),
                    Username = sqlReader["username"].ToString()
                };
                
                sqlReader.Close();

                return person;
            }
            catch (Exception e)
            {
                throw new Exception("Error retreiving person from database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public int InsertPerson(PersonModel person)
        {
            const string personQuery = "INSERT INTO [dbo].[Person]([FName],[LName],[email],[phone],[address],[UserName])"
                        + " VALUES (@FName,@LName,@email,@phone,@address,@UserName) select SCOPE_IDENTITY() as id";
            const string credsQuery = "INSERT INTO [dbo].[Credentials] ([PersonId], [Password]) VALUES (@pID, @password)";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(personQuery, conn);
                sqlCmd.Parameters.AddWithValue("@FName", person.FirstName);
                sqlCmd.Parameters.AddWithValue("@LName", person.LastName);
                sqlCmd.Parameters.AddWithValue("@email", person.Email);
                sqlCmd.Parameters.AddWithValue("@phone", person.Phone);
                sqlCmd.Parameters.AddWithValue("@address", person.Address);
                sqlCmd.Parameters.AddWithValue("@UserName", person.Username);
                
                var sqlReader = sqlCmd.ExecuteReader();
                sqlReader.Read();
                person.PersonId = Convert.ToInt32(sqlReader[0].ToString());
                sqlReader.Close();

                // Insert into Credentials table
                sqlCmd = new SqlCommand(credsQuery, conn);
                sqlCmd.Parameters.AddWithValue("@pID", person.PersonId);
                sqlCmd.Parameters.AddWithValue("@password", person.Password);
                sqlCmd.ExecuteNonQuery();

                return person.PersonId;
            }
            catch (Exception e)
            {
                throw new Exception("Error inserting person into to database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        private SqlConnection GetConnection(string connKey)
        {
            // Get Connection
            var connStr = _configuration.GetConnectionString(connKey);

            if (string.IsNullOrEmpty(connStr))
            {
                throw new ArgumentNullException("connStr", "SqlConnection string is null.");
            }

            // Get Sql Connection
            return new SqlConnection(connStr);
        }
    }
}
