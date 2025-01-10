using System.Data;
using Microsoft.Data.SqlClient;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Data.DAO
{
    public class ProductDAO
    {
        Database db = null;
        public ProductDAO()
        {
            db = new Database();
        }

        public Products GetAllInventory() { 
            Products products = new Products();
            if (db.IsConnect())
            {
                string query = "SELECT * FROM dbo.Inventory;";
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product(
                                    reader.GetString(1),//serial
                                    reader.GetString(2),//sku
                                    reader.GetString(3),//make
                                    reader.GetString(4),//model
                                    (ushort)reader.GetInt32(5),//quanity
                                    reader.GetString(6),//description
                                    reader.GetString(7),//location
                                    reader.GetDateTime(8),//createdTimeStamp
                                    reader.GetString(9)//userTimeStamp
                                    );
                                Products.products.Add(product);
                            }

                            return products;
                        }
                    }
                }
            }
            else {
                return null;
            }

        }

        public int IncomingProductStatus(string serial, string sku, string make, string model)
        {
            var dbCon = Database.Instance();
    
            if (dbCon.IsConnect())
            {

                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateInventory"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.Parameters.Add(new SqlParameter("@p_serial", serial));
                        cmd.Parameters.Add(new SqlParameter("@p_sku", sku));
                        cmd.Parameters.Add(new SqlParameter("@p_make", make));
                        cmd.Parameters.Add(new SqlParameter("@p_model", model));

                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();
                        int result = (int)returnParameter.Value;
                        return result;
                    }
                }
            }
            else
            {
                return -1;
            }
        }

        public bool UpdateQuantity_PK_Exists(string serial, string sku, string make, string model,
           ushort quantity, DateTime time, string user)
        {
            var dbCon = Database.Instance();
            int q = Convert.ToInt32(quantity);
            if (dbCon.IsConnect())
            {
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateQuantity_PK_Exists"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.Parameters.Add(new SqlParameter("@p_serial", serial));
                        cmd.Parameters.Add(new SqlParameter("@p_sku", sku));
                        cmd.Parameters.Add(new SqlParameter("@p_make", make));
                        cmd.Parameters.Add(new SqlParameter("@p_model", model));
                        cmd.Parameters.Add(new SqlParameter("@p_quantity", q));
                        cmd.Parameters.Add(new SqlParameter("@p_time", time));
                        cmd.Parameters.Add(new SqlParameter("@p_user", user));


                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public bool InsertProduct(string serial, string sku, string make, string model,
            ushort quantity, string description, string location, DateTime createdTime, string userId)
        {
            var dbCon = Database.Instance();
            int q = Convert.ToInt32(quantity);
            if (dbCon.IsConnect())
            {

                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("Insert_Product"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.Parameters.Add(new SqlParameter("@p_serial", serial));
                        cmd.Parameters.Add(new SqlParameter("@p_sku", sku));
                        cmd.Parameters.Add(new SqlParameter("@p_make", make));
                        cmd.Parameters.Add(new SqlParameter("@p_model", model));
                        cmd.Parameters.Add(new SqlParameter("@p_quantity", q));
                        cmd.Parameters.Add(new SqlParameter("@p_description", description));
                        cmd.Parameters.Add(new SqlParameter("@p_location", location));
                        cmd.Parameters.Add(new SqlParameter("@p_timeStamp", createdTime));
                        cmd.Parameters.Add(new SqlParameter("@p_userStamp", userId));

                        cmd.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        public void UpdateProduct(Product p)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("UPDATE dbo.Inventory SET quantity = {4}, description = '{5}'," +
                    "location = '{6}' WHERE serial = '{0}' AND sku = '{1}' AND make = '{2}' AND model = '{3}';",
                    p.serial, p.sku, p.make, p.model, p.quantity, p.description, p.location);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteScalar();

                    }
                }
            }

        }

        public void DeleteProduct(Product p)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("DELETE FROM dbo.Inventory WHERE serial = '{0}' " +
                    "AND sku = '{1}' AND make = '{2}' AND model = '{3}';", p.serial, p.sku, p.make,
                    p.model);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteScalar();
                        //conn.Close();
                    }

                }
            }
        }

        public DataTable GetProductFromSKU(string sku)
        {
            var dbCon = Database.Instance();
            DataTable dt = new DataTable();
            dt.Columns.Add("Serial Number", typeof(string));
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Quantity", typeof(ushort));
            dt.Columns.Add("Short Description", typeof(string));
            dt.Columns.Add("Location", typeof(string));
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Inventory WHERE SKU = '{0}';", sku);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int i = 0;
                            while (reader.Read())
                            {
                                Product p = new Product(reader.GetInt32(0),reader.GetString(1), reader.GetString(2),
                                    reader.GetString(3), reader.GetString(4), (ushort)reader.GetInt32(5),
                                    reader.GetString(6), reader.GetString(7),reader.GetDateTime(8),reader.GetString(9));
                                DataRow drow = dt.NewRow();
                                dt.Rows.Add(drow);
                                dt.Rows[i]["Serial Number"] = p.serial;
                                dt.Rows[i]["SKU"] = p.sku;
                                dt.Rows[i]["Make"] = p.make;
                                dt.Rows[i]["Model"] = p.model;
                                dt.Rows[i]["Quantity"] = p.quantity;
                                dt.Rows[i]["Short Description"] = p.description;
                                dt.Rows[i]["Location"] = p.location;

                                i++;
                            }


                        }
                        //conn.Close();

                    }
                }


                //dbCon.Close();
                return dt;
            }
            return null;


        }

        public DataTable GetProductFromSerial(string serial)
        {
            var dbCon = Database.Instance();
            DataTable dt = new DataTable();
            dt.Columns.Add("Serial Number", typeof(string));
            dt.Columns.Add("SKU", typeof(string));
            dt.Columns.Add("Make", typeof(string));
            dt.Columns.Add("Model", typeof(string));
            dt.Columns.Add("Quantity", typeof(ushort));
            dt.Columns.Add("Short Description", typeof(string));
            dt.Columns.Add("Location", typeof(string));
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Inventory WHERE serial = '{0}';", serial);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            int i = 0;
                            while (reader.Read())
                            {
                                Product p = new Product(reader.GetString(1), reader.GetString(2),
                                    reader.GetString(3), reader.GetString(4), (ushort)reader.GetInt32(5),
                                    reader.GetString(6), reader.GetString(7));
                                DataRow drow = dt.NewRow();
                                dt.Rows.Add(drow);
                                dt.Rows[i]["Serial Number"] = p.serial;
                                dt.Rows[i]["SKU"] = p.sku;
                                dt.Rows[i]["Make"] = p.make;
                                dt.Rows[i]["Model"] = p.model;
                                dt.Rows[i]["Quantity"] = p.quantity;
                                dt.Rows[i]["Short Description"] = p.description;
                                dt.Rows[i]["Location"] = p.location;

                                i++;
                            }


                        }
                        //conn.Close();

                    }
                }


                //dbCon.Close();
                return dt;
            }
            return null;


        }

        public DataTable GetModelsFromMake(string make)
        {
            var dbCon = Database.Instance();
            DataTable dt = new DataTable();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT model FROM dbo.Inventory WHERE make = '{0}';", make);
                using (SqlConnection conn = new SqlConnection(dbCon.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        SqlDataAdapter adr = new SqlDataAdapter(query, conn);
                        adr.SelectCommand.CommandType = CommandType.Text;
                        adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                    }
                }


                // dbCon.Close();
                return dt;
            }
            return null;
        }

        public Product GetProductToUpdate(Product p) {
            Product product = new Product();
            if (db.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Inventory WHERE serial = '{0}' AND sku = '{1}' AND make = '{2}' AND model = '{3}';",
                    p.serial,p.sku,p.make,p.model);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                product = new Product(
                                    reader.GetInt32(0),//id
                                    reader.GetString(1),//serial
                                    reader.GetString(2),//sku
                                    reader.GetString(3),//make
                                    reader.GetString(4),//model
                                    (ushort)reader.GetInt32(5),//quanity
                                    reader.GetString(6),//description
                                    reader.GetString(7),//location
                                    reader.GetDateTime(8),//createdTimeStamp
                                    reader.GetString(9)//userTimeStamp
                                    );
                                
                            }

                            return product;
                        }
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public ushort SetQuantity(ushort quantity, string serial, string sku, string make, string model)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("UPDATE dbo.Inventory SET quantity = {0} WHERE " +
                    "serial = '{1}' AND sku = '{2}' AND make = '{3}' AND model = '{4}';", quantity,
                    serial, sku, make, model);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.ExecuteScalar();
                        //conn.Close();
                    }
                }


                // dbCon.Close();
                return quantity;
            }
            return 0;


        }

        public List<string> GetListOfBrands()
        {
            var dbCon = Database.Instance();
            List<string> brands = new List<string>();

            if (dbCon.IsConnect())
            {
                using (SqlConnection connection = new SqlConnection(db.connstring))
                {
                    string query = String.Format("SELECT DISTINCT name from dbo.Brands ORDER BY name;");
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                brands.Add(reader.GetString(0));
                            }


                        }

                    }
                }
            }

            return brands;

        }

    }
}
