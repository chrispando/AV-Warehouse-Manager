using System.Data;
using SounDesign_Web_02.Data.DAO;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Services
{
    public class ProductService
    {
        SounDesign_Web_02.Data.DAO.ProductDAO productDAO;
        Database db = new Database();
        public ProductService()
        {
            productDAO = new SounDesign_Web_02.Data.DAO.ProductDAO();

        }
        public Products GetAllInventory() { 
            Products products = new Products();
            try
            {
                return productDAO.GetAllInventory();
            }
            catch (Exception ex)
            {
                return products;
            }
         
        }

        public int IncomingProductStatus(string serial, string sku, string make, string model) {

            int productStatus = -1;
            try
            {
                return productDAO.IncomingProductStatus(serial,sku,make,model);
            }
            catch (Exception ex) {
                return productStatus;
            }

            return productStatus;
        }

        public bool UpdateQuantity_PK_Exists(string serial, string sku, 
            string make, string model, ushort quantity, DateTime time, string user) {
            bool b = false;
            try
            {
                return productDAO.UpdateQuantity_PK_Exists(serial, sku, make, model, quantity,time,user);
            }
            catch (Exception ex) {
                return b;
            }
            return b;
        }

        public bool InsertProduct(string serial, string sku, string make, string model, 
            ushort quantity, string description, string location, DateTime createdTime, string userId) {
            bool b=false;
            try
            {
                return productDAO.InsertProduct(serial, sku, make, model, quantity,
                                description, location, createdTime, userId);
            }
            catch (Exception ex) {
                return b;
            }
            return b;
        }

        public void UpdateProduct(Product p)
        {
            try
            {
                productDAO.UpdateProduct(p);
            }
            catch (Exception ex) { 
                
            }
        }

        public void DeleteProduct(Product p) {
            try
            {
                productDAO.DeleteProduct(p);
            }
            catch (Exception ex)
            {

            }
        }

        public bool ContainsSKU(string sku)
        {
            try
            {
                return db.ContainsSKU(sku);
            }
            catch (Exception ex) {
                return false;
            }
        }

        public DataTable GetProductFromSKU(string sku)
        {
            try
            {
                return productDAO.GetProductFromSKU(sku);
            }
            catch (Exception ex) {
                return null;
            }
        }

        public Product GetProductFromSerial(string serial)
        {
            try
            {
                DataTable dt = productDAO.GetProductFromSerial(serial);
                DataRow dr = dt.Rows[0];
                Product product = new Product(dr[0].ToString(),
                    dr[1].ToString(), dr[2].ToString(), dr[3].ToString(),
                    (ushort)UInt32.Parse(dr[4].ToString()), dr[5].ToString(),
                    dr[6].ToString());
                return product;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetModelsFromMake(string make)
        {
            try
            {
                List<string> models = new List<string>();
                foreach (DataRow dr in productDAO.GetModelsFromMake(make).Rows)
                {
                    models.Add(dr[0].ToString());
                }
                return models;
            }
            catch (Exception ex) {
                return null;
            }
        }

        public Product GetProductToUpdate(Product p)
        {
            try
            {
                return productDAO.GetProductToUpdate(p);
            }
            catch (Exception ex) {
                return null;
            }
        }

        public List<string> GetListOfBrands()
        {
            try
            {
                return productDAO.GetListOfBrands();
            }
            catch (Exception ex) {
                return new List<string>();
            }
        }


    }
}
