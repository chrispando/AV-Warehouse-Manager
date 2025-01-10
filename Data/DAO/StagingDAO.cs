using System.Data;
using Microsoft.Data.SqlClient;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Data.DAO
{
    public class StagingDAO
    {
        Database db = null;
        ProductDAO productDAO = new SounDesign_Web_02.Data.DAO.ProductDAO();
        public StagingDAO()
        {
            db = new Database();
        }

        public Stagings GetAllStaging()
        {
            Stagings stagings = new Stagings();
            if (db.IsConnect())
            {
                string query = "SELECT * FROM dbo.Staging;";
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
                                Staging staging = new Staging(
                                    reader.GetString(1),//serial
                                    reader.GetString(2),//sku
                                    reader.GetString(3),//make
                                    reader.GetString(4),//model
                                    (ushort)reader.GetInt32(5),//quanity
                                    reader.GetString(6),//description
                                    reader.GetString(7),//site
                                    reader.GetString(8),//room
                                    reader.GetDateTime(9),//createdTimeStamp
                                    reader.GetDateTime(10),//stagedTimeStamp
                                    reader.GetString(11),//productUserStamp
                                    reader.GetString(12)//stagedUserStamp
                                    );
                                Stagings.stagings.Add(staging);
                            }

                            return stagings;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

        }

        public DataTable GridLoadProjectStaging(string p)
        {
            var dbCon = Database.Instance();
            DataTable dt = new DataTable();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Staging WHERE site = '{0}';", p);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        SqlDataAdapter adr = new SqlDataAdapter(query, conn);
                        adr.SelectCommand.CommandType = CommandType.Text;
                        adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
                    }
                }
            }

            return dt;

        }

        public List<string> GetStagingProjectNames()
        {
            var dbCon = Database.Instance();
            List<string> models = new List<string>();

            if (dbCon.IsConnect())
            {
                using (SqlConnection connection = new SqlConnection(db.connstring))
                {
                    string query = String.Format("SELECT DISTINCT site from dbo.Staging ORDER BY site;");
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                models.Add(reader.GetString(0));
                            }


                        }

                    }
                }
            }

            return models;

        }
        public List<string> GetStagingRoomNames()
        {
            var dbCon = Database.Instance();
            List<string> models = new List<string>();

            if (dbCon.IsConnect())
            {
                using (SqlConnection connection = new SqlConnection(db.connstring))
                {
                    string query = String.Format("SELECT name from dbo.Rooms;");
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                models.Add(reader.GetString(0));
                            }


                        }

                    }
                }
            }

            return models;

        }
        public bool ContainsPrimaryKey(string serial, string sku, string make, string model)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT CASE WHEN EXISTS(SELECT serial, sku, make, model FROM dbo.Inventory WHERE serial = '{0}' " +
                    "AND sku = '{1}' AND make = '{2}' AND model = '{3}') THEN 'True' ELSE 'False' END;", serial, sku, make, model);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        string val1 = cmd.ExecuteScalar().ToString().Trim();
                        //conn.Close();
                        //dbCon.Close();
                        return val1 == "True" ? true : false;
                    }
                }

            }
            else
            {
                return false;
            }


        }

        public Product GetPrimaryKey(string serial, string sku, string make, string model)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Inventory WHERE serial = '{0}' AND " +
                    "sku = '{1}' AND make = '{2}' AND model = '{3}';", serial, sku, make, model);
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
                                Product p = new Product(reader.GetString(1), reader.GetString(2),
                                    reader.GetString(3), reader.GetString(4), (ushort)reader.GetInt32(5),
                                    reader.GetString(6), reader.GetString(7), reader.GetDateTime(8),
                                    reader.GetString(9));
                                return p;
                            }


                        }
                        //conn.Close();

                    }
                }



            }
            return null;


        }

        public bool CheckForItemInStaging(Staging p)
        {
            var dbCon = Database.Instance();


            if (dbCon.IsConnect())
            {
                using (SqlConnection connection = new SqlConnection(db.connstring))
                {
                    string query = String.Format("SELECT Model FROM dbo.Staging " +
                        "WHERE serial = '{0}'" +
                        " AND sku = '{1}' AND make = '{2}' AND model = '{3}'" +
                        " AND site = '{4}' AND room = '{5}';",
                        p.serial, p.sku, p.make, p.model, p.site, p.room);
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = connection;
                        connection.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            return reader.HasRows ? true : false;

                        }

                    }
                }
            }

            return false;



        }

        public ushort GetQuantityStaging(Staging p)
        {
            var dbCon = Database.Instance();
            ushort quantity = 0;
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT quantity FROM dbo.Staging WHERE serial = '{0}' " +
                    "AND sku = '{1}' AND make = '{2}' AND model = '{3}' AND site = '{4}' " +
                    "AND room = '{5}';", p.serial, p.sku, p.make, p.model, p.site, p.room);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        quantity = (ushort)UInt32.Parse(cmd.ExecuteScalar().ToString());
                        //conn.Close();
                    }
                }


                // dbCon.Close();
                return quantity;
            }
            return 0;


        }






        public void StageOrderSummary(OrderSummary os, DateTime now, string userId)
        {
            var dbCon = Database.Instance();
            StagingDAO stagingDAO = new StagingDAO();
            string query = "";
            if (dbCon.IsConnect())
            {
                if (stagingDAO.CheckForItemInStaging(new Staging(os)))
                {
                    int quantity = stagingDAO.GetQuantityStaging(new Staging(os)) + os.required;
                    query = String.Format("UPDATE dbo.Staging SET quantity = {0}, stagedTimeStamp = '{7}', stagingUserStamp = '{8}' " +
                        "WHERE serial = '{1}' AND sku = '{2}' AND make = '{3}' AND model = '{4}' " +
                        "AND site = '{5}' AND room = '{6}';", quantity, os.serial, os.sku, os.make, os.model,
                        os.site, os.room, now, userId);


                }
                else
                {
                    query = String.Format("INSERT INTO dbo.Staging VALUES ('{0}', '{1}', " +
                    "'{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}','{11}');", 
                    os.serial, os.sku, os.make, os.model, os.required,
                    os.description, os.site, os.room, os.createdTime,now,os.createdUserStamp,userId);
                }

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

        public ushort MovePKToStaging(Staging p, string projectName, DateTime stagingTimeStamp, string stagingUserStamp)
        {
            var dbCon = Database.Instance();
            string query = "";
            if (dbCon.IsConnect())
            {
                if (CheckForItemInStaging(p))
                {
                    int quantity = GetQuantityStaging(p) + p.quantity;
                    query = String.Format("UPDATE dbo.Staging SET quantity = {0}, stagedTimeStamp = '{7}', stagingUserStamp = '{8}' " +
                        "WHERE serial = '{1}' AND sku = '{2}' AND make = '{3}' AND model = '{4}' " +
                        "AND site = '{5}' AND room = '{6}';", quantity, p.serial, p.sku, p.make, p.model,
                        p.site, p.room, stagingTimeStamp, stagingUserStamp);


                }
                else
                {
                    query = String.Format("INSERT INTO dbo.Staging VALUES ('{0}', '{1}', " +
                    "'{2}', '{3}', '{4}', '{5}', '{6}', '{7}','{8}','{9}','{10}','{11}');",
                    p.serial, p.sku, p.make, p.model, p.quantity,
                    p.description, p.site, p.room, p.createdTimeStamp, stagingTimeStamp, p.productUserStamp, stagingUserStamp);
                }


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

                if (ContainsPrimaryKey("None", p.sku, p.make, p.model))
                {
                    Product deleteThis = new Product("None", p.sku, p.make, p.model,
                        p.quantity, p.description, "");

                    productDAO.DeleteProduct(deleteThis);

                    query = String.Format("UPDATE dbo.Inventory " +
                            "SET serial = 'None', quantity = '0' WHERE serial = '{0}' " +
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

                else
                {
                    query = String.Format("UPDATE dbo.Inventory " +
                            "SET serial = 'None', quantity = 0 WHERE serial = '{0}' " +
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
            return 0;


        }

        public void StageOrderSummaryPK(OrderSummary os, DateTime stagingTimeStamp, string stagingUserStamp)
        { 
            MovePKToStaging(new Staging(os), os.site, stagingTimeStamp, stagingUserStamp);

        }

        public string GetStagingDescription(string serial, string sku, string make, string model)
        {
            var dbCon = Database.Instance();
            string description = "";
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT description FROM dbo.Inventory WHERE serial = '{0}' " +
                    "AND sku = '{1}' AND make = '{2}' AND model = '{3}';",
                    serial, sku, make, model);
                using (SqlConnection conn = new SqlConnection(db.connstring))
                {
                    using (SqlCommand cmd = new SqlCommand(query))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();
                        try
                        {
                            if (ContainsPrimaryKey(serial, sku, make, model))
                            {
                                description = cmd.ExecuteScalar().ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            description = "";
                        }

                        //quantity = (ushort)UInt32.Parse(cmd.ExecuteScalar().ToString());
                        //conn.Close();
                    }
                }


                // dbCon.Close();

            }
            return description;


        }

        public void DeleteProductStaging(string serial, string sku, string make, string model, string site, string room)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("DELETE FROM dbo.Staging WHERE serial = '{0}' " +
                    "AND sku = '{1}' AND make = '{2}' AND model = '{3}' AND site = '{4}' AND room = '{5}';", serial, sku, make,
                    model, site, room);
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

        public void UpdateQuantityStaging(ushort quantity, string serial, string sku, string make, string model, string site, string room)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("UPDATE dbo.Staging SET quantity = {0} WHERE serial = '{1}' " +
                    "AND sku = '{2}' AND make = '{3}' AND model = '{4}' AND site = '{5}' " +
                    "AND room = '{6}';", quantity, serial, sku, make, model, site, room);
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

        public Staging GetStagingItem(string serial, string sku, string make, string model, string site, string room)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Staging WHERE serial = '{0}' AND " +
                    "sku = '{1}' AND make = '{2}' AND model = '{3}' AND site = '{4}' AND room = '{5}';", 
                    serial, sku, make, model, site, room);
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
                                Staging p = new Staging(
                                    reader.GetString(1), //serial
                                    reader.GetString(2), //sku
                                    reader.GetString(3), //make
                                    reader.GetString(4), //model
                                    (ushort)reader.GetInt32(5),//quantity
                                    reader.GetString(6), //description
                                    reader.GetString(7), //site
                                    reader.GetString(8), //room
                                    reader.GetDateTime(9),//createdTimeStamp
                                    reader.GetDateTime(10),//stagedTimeStamp
                                    reader.GetString(11), //productUserStamp
                                    reader.GetString(12));//stagedUserStamp
                                return p;
                            }


                        }
                        //conn.Close();

                    }
                }



            }
            return null;


        }


    }
}
