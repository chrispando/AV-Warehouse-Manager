using System.Data;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Services
{
    public class TruckService
    {
        SounDesign_Web_02.Data.DAO.TruckDAO truckDAO;
        public TruckService()
        {
            truckDAO = new SounDesign_Web_02.Data.DAO.TruckDAO();

        }
        public Trucks GetAllTruck()
        {
            Trucks trucks = new Trucks();
            try
            {
                return truckDAO.GetAllTruck();
            }
            catch (Exception ex)
            {
                return trucks;
            }

        }

        public void InsertProduct(Truck item,DateTime truckTimeStamp, string truckUserStamp)
        {
            try
            {
                truckDAO.InsertProduct(item,truckTimeStamp,truckUserStamp);
            }
            catch (Exception ex) { 
            
            }
        }

        public DataTable GridLoadProjectTruck(string id)
        {
            try
            {
                return truckDAO.GridLoadProjectTruck(id);
            }
            catch (Exception ex) {
                return new DataTable();
            }
        
        }

        public List<string> GetTruckProjectNames()
        {
            try
            {
                return truckDAO.GetTruckProjectNames();
            }
            catch (Exception ex) {
                return new List<string>();
            }
        }

        public Truck GetTruckItem(string serial, string sku, string make, string model,string site, string room)
        {
            try
            {
                return truckDAO.GetTruckItem(serial, sku, make, model,site,room);
            }
            catch (Exception ex) {
                return null;
            }
        }

        public void DeleteFromTruck(string serial, string sku, string make, string model, string site, string room)
        {
            try
            {
                truckDAO.DeleteFromTruck(serial, sku, make, model, site, room);
            }
            catch (Exception ex) { 
            
            }
        }
    }
}
