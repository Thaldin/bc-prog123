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

        public PersonDAL(IConfiguration config)
        {
            _configuration = config;
        }

        public int InsertPerson(PersonModel person)
        {
            const string personQuery = "INSERT INTO [dbo].[Person]([FName],[LName],[email],[phone],[address],[UserName])"
                        + " VALUES (@FName,@LName,@email,@phone,@address,@UserName) select SCOPE_IDENTITY() as id";
            const string credsQuery = "INSERT INTO [dbo].[Credentials] ([PersonId], [Password]) VALUES (@pID, @password)";

            SqlConnection conn = null;

            try
            {
                // Get Connection
                var connStr = _configuration.GetConnectionString("MyConnString");

                if (string.IsNullOrEmpty(connStr))
                {
                    throw new ArgumentNullException("connStr", "SqlConnection string is null.");
                }

                // Get Sql Connection
                conn = new SqlConnection(connStr);
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
    }
}
