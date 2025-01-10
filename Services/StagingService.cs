using System.Data;
using SounDesign_Web_02.Data.DAO;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Services
{
    public class StagingService
    {
        SounDesign_Web_02.Data.DAO.StagingDAO stagingDAO;
        ProductDAO productDAO = new ProductDAO();
        public StagingService()
        {
            stagingDAO = new SounDesign_Web_02.Data.DAO.StagingDAO();

        }
        public Stagings GetAllStaging()
        {
            Stagings stagings = new Stagings();
            try
            {
                return stagingDAO.GetAllStaging();
            }
            catch (Exception ex)
            {
                return stagings;
            }

        }

        public DataTable GridLoadProjectStaging(string id)
        {
            try
            {
                return stagingDAO.GridLoadProjectStaging(id);
            }
            catch (Exception ex) {
                return null;
            }
        }

        public List<string> GetStagingProjectNames()
        {
            try
            {
                return stagingDAO.GetStagingProjectNames();
            }
            catch (Exception ex) { 
                return new List<string>();  
            }
        }
        public List<string> GetStagingRoomNames()
        {
            try
            {
                return stagingDAO.GetStagingRoomNames();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
        public bool ContainsPrimaryKey(string serial, string sku, string make, string model)
        {
            try
            {
                return stagingDAO.ContainsPrimaryKey(serial, sku, make, model);
            }
            catch (Exception ex) { 
                return false;
            }
        }

        public Product GetPrimaryKey(string serial, string sku, string make, string model)
        {
            try
            {
                return stagingDAO.GetPrimaryKey(serial, sku, make, model);
            }
            catch (Exception ex) {
                return null;
            }
        }

        public void StageOrderSummary(OrderSummary os, DateTime stagingTimeStamp, string stagingUserStamp)
        {
            try
            {
                stagingDAO.StageOrderSummary(os, stagingTimeStamp,stagingUserStamp );
            }
            catch (Exception ex) { 
            
            }
        }
        public void StageOrderSummaryPK(OrderSummary os, DateTime stagingTimeStamp, string stagingUserStamp)
        {
            try
            {
                stagingDAO.StageOrderSummaryPK(os, stagingTimeStamp, stagingUserStamp);
            }
            catch (Exception ex)
            {

            }
        }
        public void SetQuantity(ushort leftInStock, string serial, string sku, string make, string model)
        {
            try
            {
                productDAO.SetQuantity(leftInStock, serial, sku, make,model);
            }
            catch (Exception ex)
            {

            }
        }

        public string GetStagingDescription(string serial, string sku, string make, string model)
        {
            try
            {
                return stagingDAO.GetStagingDescription(serial, sku, make, model);
            }
            catch (Exception ex) {
                return null;
            }
        }

        public void DeleteProductStaging(string serial, string sku, string make, string model, string site, string room) {
            try
            {
                stagingDAO.DeleteProductStaging(serial, sku, make, model, site, room);
            }
            catch (Exception ex) { 
                
            }
        
        }

        public void UpdateQuantityStaging(ushort newQuantity, string serial, string sku, string make, string model, string site, string room)
        {
            try
            {
                stagingDAO.UpdateQuantityStaging(newQuantity, serial, sku, make, model, site, room);
            }
            catch (Exception ex)
            {
            }
        }

        public Staging GetStagingItem(string serial, string sku, string make, string model, string site, string room)
        {
            try
            {
                return stagingDAO.GetStagingItem(serial, sku, make, model, site, room);
            }
            catch (Exception ex) {
                return null;
            }
        }


    }
}
