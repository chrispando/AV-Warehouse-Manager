using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SounDesign_Web_02.Models;
using SounDesign_Web_02.Services;

namespace SounDesign_Web_02.Controllers
{
    [Authorize]
    public class TruckController : Controller
    {
        private SounDesign_Web_02.Services.TruckService truckService = new SounDesign_Web_02.Services.TruckService();
        StagingService stagingService = new StagingService();

        [HttpPost]
        public IActionResult PutOnTruck([FromBody] string t)
        {
            List<Truck> truck = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Truck>>(t);
            Data.DAO.StagingDAO db = new Data.DAO.StagingDAO();
            Data.DAO.TruckDAO truckDAO = new Data.DAO.TruckDAO();
            foreach (Truck item in truck)
            {
                ushort quantityStaged = 0;
                ushort quantityTruck = 0;
                Staging s2 = stagingService.GetStagingItem(item.serial, item.sku, item.make, item.model, item.site, item.room);
                quantityStaged = db.GetQuantityStaging(item.ToStaging());
                quantityTruck = item.quantity;
                try
                {
                    item.createdTimeStamp = s2.createdTimeStamp;
                    item.stagedTimeStamp = s2.stagedTimeStamp;
                    item.productUserStamp = s2.productUserStamp;
                    item.stagingUserStamp = s2.stagingUserStamp;
                }
                catch (Exception ex) { 
                    
                }
                
                if (quantityStaged == quantityTruck)
                {
                    stagingService.DeleteProductStaging(item.serial, item.sku, item.make, item.model, item.site, item.room);
                    truckService.InsertProduct(item,DateTime.Now,User.Identity.Name.ToString());
                    continue;
                }
                else if (quantityStaged > quantityTruck)
                {
                    ushort newQuantity = (ushort)(quantityStaged - quantityTruck);
                    stagingService.UpdateQuantityStaging(newQuantity, item.serial, item.sku, item.make, item.model, item.site, item.room);
                    truckService.InsertProduct(item, DateTime.Now, User.Identity.Name.ToString());
                    continue;
                }
                else
                {
                    return Json(new { status = "false", msg = "Not enough inventory" });
                }
            }
            return Json(new { status = "true", msg = "Redirect now" });
        }
        public IActionResult SelectForTruck()
        {

            List<Staging> myModel = new List<Staging>();
            if (HttpContext.Session.Get("stagingProjectModel") != null)
            {
                //Cast your session variable as required
                myModel = (List<Staging>)(HttpContext.Session.GetObject<List<Staging>>("stagingProjectModel"));
            }

            return View(myModel);
        }
        public IActionResult Create()
        {
            var model = stagingService.GetStagingProjectNames();

            return View(model);
        }
        public IQueryable<Truck> ToList(DataTable dt)
        {
            List<Truck> products = new List<Truck>();
            foreach (DataRow dr in dt.Rows)
            {
                Truck product = new Truck();
                product.serial = dr[1].ToString();
                product.sku = dr[2].ToString();
                product.make = dr[3].ToString();
                product.model = dr[4].ToString();
                product.quantity = (ushort)UInt32.Parse(dr[5].ToString());
                product.description = dr[6].ToString();
                product.site = dr[7].ToString();
                product.room = dr[8].ToString();
                product.technician = dr[9].ToString();
                product.signature = dr[10].ToString();
                product.createdTimeStamp = (DateTime)dr[11];
                product.stagedTimeStamp = (DateTime)dr[12];
                product.truckTimeStamp = (DateTime)dr[13];
                product.productUserStamp = dr[14].ToString();
                product.stagingUserStamp = dr[15].ToString();
                product.truckUserStamp = dr[16].ToString();
                products.Add(product);
            }
            return products.AsQueryable<Truck>();
        }

        public IQueryable<Staging> ToListStaging(DataTable dt)
        {
            List<Staging> products = new List<Staging>();
            foreach (DataRow dr in dt.Rows)
            {
                Staging product = new Staging();
                product.serial = dr[1].ToString();
                product.sku = dr[2].ToString();
                product.make = dr[3].ToString();
                product.model = dr[4].ToString();
                product.quantity = (ushort)UInt32.Parse(dr[5].ToString());
                product.description = dr[6].ToString();
                product.site = dr[7].ToString();
                product.room = dr[8].ToString();
                product.createdTimeStamp = (DateTime)dr[9];
                product.stagedTimeStamp = (DateTime)dr[10];
                product.productUserStamp = dr[11].ToString();
                product.stagingUserStamp = dr[12].ToString();

                products.Add(product);
            }
            return products.AsQueryable<Staging>();
        }


        [HttpPost]
        public IActionResult ProjectSelected([FromBody] string p)
        {
            Data.DAO.TruckDAO _db = new Data.DAO.TruckDAO();
            var myJsonString = p;
            var jo = JObject.Parse(myJsonString);
            var id = jo["projectName"].ToString();

            var model = ToList(truckService.GridLoadProjectTruck(id));
            HttpContext.Session.SetObject("truckProjectModel", model);
            HttpContext.Session.SetObject("mostRecentTruckName", model.First().site);

            return Json(new { status = "true", msg = "Redirect now" });
        }

        [HttpPost]
        public IActionResult ProjectSelected_StagingList([FromBody] string p)
        {
            Data.DAO.StagingDAO _db = new Data.DAO.StagingDAO();
            var myJsonString = p;
            var jo = JObject.Parse(myJsonString);
            var id = jo["projectName"].ToString();

            var model = ToListStaging(_db.GridLoadProjectStaging(id));
            HttpContext.Session.SetObject("stagingProjectModel", model);

            return Json(new { status = "true", msg = "Redirect now" });
        }


        public async Task<IActionResult> DisplayProject(string sortOrder, string searchString, int pg = 1, int pageSize = 6)
        {
            IEnumerable<Truck> products = null;
            if (HttpContext.Session.Get("truckProjectModel") != null)
            {
                //Cast your session variable as required
                //products = (IEnumerable<Truck>)(HttpContext.Session.GetObject<IEnumerable<Truck>>("truckProjectModel"));
                products = (IEnumerable<Truck>)truckService.GridLoadProjectTruck(HttpContext.Session.GetObject<string>("mostRecentTruckName")).AsEnumerable().Select(row => new Truck
                {
                    serial = Convert.ToString(row["serial"]),
                    sku = Convert.ToString(row["sku"]),
                    make = Convert.ToString(row["make"]),
                    model = Convert.ToString(row["model"]),
                    quantity = (ushort)Convert.ToUInt32(row["quantity"]),
                    description = Convert.ToString(row["description"]),
                    site = row["site"].ToString(),
                    room = Convert.ToString(row["room"]),
                    technician = Convert.ToString(row["technician"]),
                    signature = Convert.ToString(row["signature"]),
                });
            }
           
            

            ViewData["MakeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "make_desc" : "";
            ViewData["ModelSortParm"] = sortOrder == "Model" ? "model_desc" : "Model";
            ViewData["CurrentFilter"] = searchString;

            Data.DAO.Database db = new Data.DAO.Database();


            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.make.ToLower().Contains(searchString.ToLower())
                    || p.model.ToLower().Contains(searchString.ToLower()));
            }
            if (products.Count()==0 && String.IsNullOrEmpty(searchString)) {
                products = (IEnumerable<Truck>)truckService.GridLoadProjectTruck(HttpContext.Session.GetObject<string>("mostRecentTruckName")).AsEnumerable().Select(row => new Truck
                {
                    serial = Convert.ToString(row["serial"]),
                    sku = Convert.ToString(row["sku"]),
                    make = Convert.ToString(row["make"]),
                    model = Convert.ToString(row["model"]),
                    quantity = (ushort)Convert.ToUInt32(row["quantity"]),
                    description = Convert.ToString(row["description"]),
                    site = row["site"].ToString(),
                    room = Convert.ToString(row["room"]),
                    technician = Convert.ToString(row["technician"]),
                    signature = Convert.ToString(row["signature"]),
                });
            }
            switch (sortOrder)
            {
                case "make_desc":
                    products = products.OrderByDescending(s => s.make);
                    break;
                case "Model":
                    products = products.OrderBy(s => s.model);
                    break;
                case "model_desc":
                    products = products.OrderByDescending(s => s.model);
                    break;
                default:
                    products = products.OrderBy(s => s.make);
                    break;
            }
            if (pg < 1)
                pg = 1;
            int recsCount = products.Count();
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = products.Skip(recSkip).Take(pager.PageSize).ToList();
            this.ViewBag.Pager = pager;
            // return View(products);

            this.ViewBag.PageSize = pageSize;
            HttpContext.Session.SetObject("truckProjectModel", products);
            return View(data);
        }
        public IActionResult Read(string searchString)
        {
            var model = truckService.GetTruckProjectNames();
            var names = model.AsQueryable<string>();
            if (!String.IsNullOrEmpty(searchString))
            {
                names = names.Where(p=>p.ToLower().Contains(searchString.ToLower()));
            }

            return View(names);

        }
        //public IActionResult Read()
        //{
        //    var model = truckService.GetTruckProjectNames();

        //    return View(model);
        //}
        public IActionResult Update()
        {
            return View();
        }
        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetUpdateSessionDetailPageTruck([FromBody] string p)
        {
            Truck product = Newtonsoft.Json.JsonConvert.DeserializeObject<Truck>(p);
            product = truckService.GetTruckItem(product.serial,product.sku,product.make,product.model,
                product.site,product.room);
            if (product is null || !ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Product not valid." });
            }
            else
            {

                HttpContext.Session.SetObject("updateThisDetailPageTruck", product);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult DetailUpdatePageTruck()
        {
            Truck updateThis = new Truck();
            if (HttpContext.Session.Get("updateThisDetailPageTruck") != null)
            {
                //Cast your session variable as required
                updateThis = (Truck)(HttpContext.Session.GetObject<Truck>("updateThisDetailPageTruck"));
            }


            return View(updateThis);
        }

        [HttpPost]
        public IActionResult SetDeleteSessionTruck([FromBody] string p)
        {
            Truck product = Newtonsoft.Json.JsonConvert.DeserializeObject<Truck>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                HttpContext.Session.SetObject("deleteThisTruck", product);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult DetailDeleteTruck()
        {
            Truck updateThis = new Truck();
            if (HttpContext.Session.Get("deleteThisTruck") != null)
            {
                //Cast your session variable as required
                updateThis = (Truck)(HttpContext.Session.GetObject<Truck>("deleteThisTruck"));
            }



            return View(updateThis);
        }

        [HttpPost]
        public IActionResult FinalDeleteTruck([FromBody] string p)
        {
            Truck product = Newtonsoft.Json.JsonConvert.DeserializeObject<Truck>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {
                truckService.DeleteFromTruck(product.serial, product.sku, product.make, product.model,
                    product.site, product.room);

                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        [HttpPost]
        public IActionResult MoveTruckToStaging([FromBody] string p) {
            Truck product = Newtonsoft.Json.JsonConvert.DeserializeObject<Truck>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {
                stagingService.StageOrderSummary(new OrderSummary(product.serial,product.sku,product.make,
                    product.model,product.description,product.site,product.room,product.createdTimeStamp,
                    product.quantity,product.quantity,"Success",product.productUserStamp),
                    DateTime.Now,User.Identity.Name.ToString());

                return Json(new { status = "true", msg = "Redirect now" });
            }
        }
    }
}
