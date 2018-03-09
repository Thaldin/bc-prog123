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

        public CompactPersonModel CheckUser(string userName, string pwd)
        {
            const string checkUser = "SELECT Person.PersonId, Person.FName FROM dbo.Person INNER JOIN Credentials ON dbo.Person.PersonID = dbo.Credentials.PersonID" +
                                     " WHERE(dbo.Person.UserName = @user) AND(dbo.Credentials.Password = @pwd)";

            SqlConnection conn = null;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
            {
                return null;
            }

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Check User
                var sqlCmd = new SqlCommand(checkUser, conn);
                sqlCmd.Parameters.AddWithValue("@user", userName);
                sqlCmd.Parameters.AddWithValue("@pwd", pwd);

                var sqlReader = sqlCmd.ExecuteReader();

                CompactPersonModel personData = null;

                if (sqlReader.HasRows)
                {
                    sqlReader.Read();

                    personData = new CompactPersonModel()
                    {
                        FirstName = sqlReader["FName"].ToString(),
                        PersonId = Convert.ToInt32(sqlReader["PersonId"])
                    };

                    sqlReader.Close();
                }

                return personData;
            }
            catch (Exception e)
            {
                throw new Exception("Error retreiving credentials from the database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void DeletePerson(string id)
        {
            const string personDelete = "DELETE FROM dbo.Person WHERE [PersonID] = @PersonId ";
            const string credsDelete = "DELETE FROM dbo.Credentials WHERE [PersonID] = @PersonId ";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Delete Credentials
                var sqlCmd = new SqlCommand(credsDelete, conn);
                sqlCmd.Parameters.AddWithValue("@PersonId", Convert.ToInt32(id));
                sqlCmd.ExecuteNonQuery();

                // Delete Person
                sqlCmd = new SqlCommand(personDelete, conn);
                sqlCmd.Parameters.AddWithValue("@PersonId", Convert.ToInt32(id));
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Error delting record from the database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void UpdatePerson(PersonModel person)
        {
            const string personUpdate = "UPDATE [dbo].[Person] SET [FName] = @FName, [LName] = @LName, [email] = @email, [phone] = @phone, [address] = @address, [UserName] = @userName WHERE [PersonId] = @id";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(personUpdate, conn);
                sqlCmd.Parameters.AddWithValue("@FName", person.FirstName);
                sqlCmd.Parameters.AddWithValue("@LName", person.LastName);
                sqlCmd.Parameters.AddWithValue("@email", person.Email);
                sqlCmd.Parameters.AddWithValue("@phone", person.Phone);
                sqlCmd.Parameters.AddWithValue("@address", person.Address);
                sqlCmd.Parameters.AddWithValue("@userName", person.Username);
                sqlCmd.Parameters.AddWithValue("@id", person.PersonId.ToString());

                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Error updating person in the database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
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
                    Username = sqlReader["username"].ToString(),
                    PersonId = (int)sqlReader["PersonId"]
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
