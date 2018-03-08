using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using Abshire_Ed.Models;
using System.Data;

namespace Abshire_Ed.DAL
{
    public class TransactionDAL
    {
        private readonly IConfiguration _configuration;
        const string _connStrKey = "MyConnString";

        public TransactionDAL(IConfiguration config)
        {
            _configuration = config;
        }

        public int InsertTransaction(SaleTransactionModel sale)
        {
            const string saleInsert = "INSERT INTO [dbo].[SalesTransaction] ([ProductID], [PersonID], [SalesDataTime], [PQuantity])"
                + " VALUES (@ProductId, @PersonId, @SaleTime, @Quantity) select SCOPE_IDENTITY() as id";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(saleInsert, conn);
                sqlCmd.Parameters.AddWithValue("@ProductId", sale.ProductId);
                sqlCmd.Parameters.AddWithValue("@PersonId", sale.PersonId);
                sqlCmd.Parameters.AddWithValue("@SaleTime", sale.TransactionTime);
                sqlCmd.Parameters.AddWithValue("@Quantity", sale.Quantity);

                var sqlReader = sqlCmd.ExecuteReader();
                sqlReader.Read();
                var saleId = Convert.ToInt32(sqlReader[0].ToString());
                sqlReader.Close();

                return saleId;
            }
            catch (Exception e)
            {
                throw new Exception("Error inserting transaction into database: " + e.Message, e);
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
