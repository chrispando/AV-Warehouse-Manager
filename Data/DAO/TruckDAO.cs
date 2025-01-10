using System.Data;
using Microsoft.Data.SqlClient;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Data.DAO
{
    public class TruckDAO
    {
        Database db = null;
        public TruckDAO()
        {
            db = new Database();
        }

        public Trucks GetAllTruck()
        {
            Trucks trucks = new Trucks();
            if (db.IsConnect())
            {
                string query = "SELECT * FROM dbo.Truck;";
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
                                Truck truck = new Truck(
                                    reader.GetString(1),//serial
                                    reader.GetString(2),//sku
                                    reader.GetString(3),//make
                                    reader.GetString(4),//model
                                    (ushort)reader.GetInt32(5),//quanity
                                    reader.GetString(6),//description
                                    reader.GetString(7),//site
                                    reader.GetString(8),//room
                                    reader.GetString(9),//technician
                                    reader.GetString(10),//signature
                                    reader.GetDateTime(11),//createdTimeStamp
                                    reader.GetDateTime(12),//stagedTimeStamp
                                    reader.GetDateTime(13),//truckTimeStamp
                                    reader.GetString(14),//productUserStamp
                                    reader.GetString(15),//stagedUserStamp
                                    reader.GetString(16)//truckUserStamp
                                    );
                                Trucks.trucks.Add(truck);
                            }

                            return trucks;
                        }
                    }
                }
            }
            else
            {
                return null;
            }

        }

        public void InsertProduct(Truck i,DateTime truckTimeStamp,string truckUserStamp)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("INSERT INTO dbo.Truck VALUES ('{0}', '{1}', " +
                    "'{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}', " +
                    "'{10}', '{11}', '{12}', '{13}', '{14}', '{15}');", i.serial, i.sku, i.make,
                    i.model, i.quantity, i.description, i.site, i.room, i.technician, i.signature,
                    i.createdTimeStamp,i.stagedTimeStamp,truckTimeStamp,i.productUserStamp,
                    i.stagingUserStamp,truckUserStamp);
                using (SqlConnection conn = new SqlConnection(dbCon.connstring))
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

        public DataTable GridLoadProjectTruck(string p)
        {
            var dbCon = Database.Instance();
            DataTable dt = new DataTable();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Truck WHERE site = '{0}';", p);
                using (SqlConnection conn = new SqlConnection(dbCon.connstring))
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

        public List<string> GetTruckProjectNames()
        {
            var dbCon = Database.Instance();
            List<string> models = new List<string>();

            if (dbCon.IsConnect())
            {
                using (SqlConnection connection = new SqlConnection(dbCon.connstring))
                {
                    string query = String.Format("SELECT DISTINCT site from dbo.Truck ORDER BY site;");
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

        public Truck GetTruckItem(string serial, string sku, string make, string model, string site, string room)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("SELECT * FROM dbo.Truck WHERE serial = '{0}' AND " +
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
                                Truck p = new Truck(
                                    reader.GetString(1), //serial
                                    reader.GetString(2), //sku
                                    reader.GetString(3), //make
                                    reader.GetString(4), //model
                                    (ushort)reader.GetInt32(5),//quantity
                                    reader.GetString(6), //description
                                    reader.GetString(7), //site
                                    reader.GetString(8), //room
                                    reader.GetString(9), //technician
                                    reader.GetString(10), //signature
                                    reader.GetDateTime(11),//createdTimeStamp
                                    reader.GetDateTime(12),//stagedTimeStamp
                                    reader.GetDateTime(13),//truckTimeStamp
                                    reader.GetString(14), //productUserStamp
                                    reader.GetString(15),//stagedUserStamp
                                    reader.GetString(16));//truckUserStamp
                                return p;
                            }


                        }
                        //conn.Close();

                    }
                }



            }
            return null;


        }

        public void DeleteFromTruck(string serial, string sku, string make, string model, string site, string room)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                string query = String.Format("DELETE FROM dbo.Truck WHERE serial = '{0}' " +
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
    }
}
