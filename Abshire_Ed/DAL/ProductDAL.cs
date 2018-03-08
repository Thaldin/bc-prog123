using Abshire_Ed.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Abshire_Ed.DAL
{
    public class ProductDAL
    {
        private readonly IConfiguration _configuration;

        const string _connStrKey = "MyConnString";

        public ProductDAL(IConfiguration config)
        {
            _configuration = config;
        }

        public ProductModel GetProduct(int productId)
        {
            const string personSelect = "SELECT * FROM [dbo].[Products] WHERE PID = @pId";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(personSelect, conn);
                sqlCmd.Parameters.AddWithValue("@pId", productId);

                var sqlReader = sqlCmd.ExecuteReader();
                sqlReader.Read();
                var product = new ProductModel()
                {
                    Name = sqlReader["Name"].ToString(),
                    Description = sqlReader["Description"].ToString(),
                    Price = float.Parse(sqlReader["price"].ToString()),
                    InventoryAmount = Convert.ToInt32(sqlReader["InventoryAmount"].ToString()),
                    ProductId = productId
                };

                sqlReader.Close();

                return product;
            }
            catch (Exception e)
            {
                throw new Exception("Error retreiving product from database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public int InsertProduct(ProductModel product)
        {
            const string productQuery = "INSERT INTO [dbo].[Products] ([Name],[Description],[Price],[InventoryAmount])"
                        + " VALUES (@Name, @Desc, @Price, @InvAmount) select SCOPE_IDENTITY() as id";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(productQuery, conn);
                sqlCmd.Parameters.AddWithValue("@Name", product.Name);
                sqlCmd.Parameters.AddWithValue("@Desc", product.Description);
                sqlCmd.Parameters.AddWithValue("@Price", product.Price);
                sqlCmd.Parameters.AddWithValue("@InvAmount", product.InventoryAmount);

                var sqlReader = sqlCmd.ExecuteReader();
                sqlReader.Read();
                product.ProductId = Convert.ToInt32(sqlReader[0].ToString());
                sqlReader.Close();

                return product.ProductId;
            }
            catch (Exception e)
            {
                throw new Exception("Error inserting product into to database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void UpdateProduct(ProductModel product)
        {
            const string productUpdate = "UPDATE [dbo].[Products] SET [Name] = @Name, [Description] = @Desc, [Price] = @Price, [InventoryAmount] = @InvAmount WHERE [PID] = @id";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Insert into Person table
                var sqlCmd = new SqlCommand(productUpdate, conn);
                sqlCmd.Parameters.AddWithValue("@Name", product.Name);
                sqlCmd.Parameters.AddWithValue("@Desc", product.Description);
                sqlCmd.Parameters.AddWithValue("@Price", product.Price);
                sqlCmd.Parameters.AddWithValue("@InvAmount", product.InventoryAmount);
                sqlCmd.Parameters.AddWithValue("@id", product.ProductId.ToString());

                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Error updating product in the database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void DeleteProduct(string id)
        {
            const string productDelete = "DELETE FROM dbo.Products WHERE [PID] = @ProductId";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Delete Product
                var sqlCmd = new SqlCommand(productDelete, conn);
                sqlCmd.Parameters.AddWithValue("@ProductId", Convert.ToInt32(id));
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting product from the database: " + e.Message, e);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public List<ProductModel> GetAllProducts()
        {
            const string productSelect = "SELECT * FROM dbo.Products";

            SqlConnection conn = null;

            try
            {
                // Get Sql Connection
                conn = GetConnection(_connStrKey);
                conn.Open();

                // Delete Product
                var sqlCmd = new SqlCommand(productSelect, conn);
                var sqlReader = sqlCmd.ExecuteReader();

                var productList = new List<ProductModel>();

                while (sqlReader.Read())
                {
                    var product = new ProductModel()
                    {
                        Name = sqlReader["name"].ToString(),
                        Description = sqlReader["Description"].ToString(),
                        Price = float.Parse(sqlReader["Price"].ToString()),
                        InventoryAmount = (int)sqlReader["InventoryAmount"],
                        ProductId = (int)sqlReader["PID"]

                    };

                    productList.Add(product);
                }

                sqlReader.Close();

                return productList;
            }
            catch (Exception e)
            {
                throw new Exception("Error getting product list from the database: " + e.Message, e);
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
