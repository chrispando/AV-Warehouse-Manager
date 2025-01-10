using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private SounDesign_Web_02.Services.ProductService productService = new SounDesign_Web_02.Services.ProductService();
        private SounDesign_Web_02.Services.StagingService stagingService = new SounDesign_Web_02.Services.StagingService();
        private SounDesign_Web_02.Services.TruckService truckService = new SounDesign_Web_02.Services.TruckService();

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult GetData() {
            Products.products.Clear();
            Stagings.stagings.Clear();
            Trucks.trucks.Clear();

            Products products = productService.GetAllInventory();
            Stagings stagings = stagingService.GetAllStaging();
            Trucks trucks = truckService.GetAllTruck();

            List<Product> productList = new List<Product>();
            List<Staging> stagingList = new List<Staging>();
            List<Truck> truckList = new List<Truck>();

            productList = Products.products;
            stagingList = Stagings.stagings;
            truckList = Trucks.trucks;

            List<CalendarItem> calendarList = new List<CalendarItem>();

            foreach (Product p in productList) {
                CalendarItem c = new CalendarItem(p);
                calendarList.Add(c);
            }
            foreach (Staging s in stagingList)
            {
                CalendarItem c = new CalendarItem(s);
                calendarList.Add(c);
            }
            foreach (Truck t in truckList)
            {
                CalendarItem c = new CalendarItem(t);
                calendarList.Add(c);
            }


            var json = new JavaScriptSerializer().Serialize(calendarList);

            return Json(json);
        }

        
    }
}
