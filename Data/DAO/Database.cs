using System.Data;
using Microsoft.Data.SqlClient;

namespace SounDesign_Web_02.Data.DAO
{
    public class Database
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public SqlConnection Connection { get; set; }

        public string connstring = String.Format("Server=(localdb)\\mssqllocaldb;Database=aspnet-SounDesign_Web_02-6EC5A6E4-A32F-4F2B-946E-32A26EE3B1A1;Trusted_Connection=True;MultipleActiveResultSets=true");

        private static Database _instance = null;

        public Database()
        {
            this.Server = "(localdb)\\mssqllocaldb";
            this.DatabaseName = "aspnet-SounDesign_Web_02-6EC5A6E4-A32F-4F2B-946E-32A26EE3B1A1";
        }
        public static Database Instance()
        {
            if (_instance == null)
                _instance = new Database();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(DatabaseName))
                    return false;


                try
                {
                    Connection = new SqlConnection(connstring);
                    Connection.Open();
                }
                catch (Exception e)
                {
                    return false;
                }

            }

            return true;
        }

        public bool ContainsSKU(string sku)
        {
            try
            {
                string query = String.Format("SELECT 1 FROM dbo.Inventory WHERE SKU = '{0}';", sku);

              
                SqlDataReader reader = getReader(query);

                return reader.HasRows;
            }
            catch (Exception ex)
            {
                return false;
            }

           
        }

        public bool ContainsMakeModel(string make, string model)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("ContainsMakeModel"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
                        cmd.Parameters.Add(new SqlParameter("@p_make", make));
                        cmd.Parameters.Add(new SqlParameter("@p_model", model));
                        var returnParameter = cmd.Parameters.Add("@ReturnVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        cmd.ExecuteNonQuery();
                        int result = (int)returnParameter.Value;
                        return result > 0 ? true : false;

                    }
                }
            }
            else
            {
                return false;
            }
        }

        public int NumberOfMatchingMakeModel(string make, string model)
        {
            var dbCon = Database.Instance();
            if (dbCon.IsConnect())
            {
                using (SqlConnection conn = new SqlConnection(connstring))
                {
                    using (SqlCommand cmd = new SqlCommand("NumberOfMatchingMakeModel"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = conn;
                        conn.Open();
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

        public SqlDataReader getReader(string query)
        {
            var dbCon = Database.Instance();
            SqlDataReader reader = null;
            try
            {
                if (dbCon.IsConnect())
                {
                    SqlConnection conn = new SqlConnection(connstring);

                    SqlCommand cmd = new SqlCommand(query, conn);
                    //cmd.CommandType = CommandType.Text;
                    //  cmd.Connection = conn;
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    return reader;

                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
            //return null;
        }
    }
}
