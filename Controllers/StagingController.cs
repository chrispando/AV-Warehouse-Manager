using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SounDesign_Web_02.Models;
using SounDesign_Web_02.Services;

namespace SounDesign_Web_02.Controllers
{
    [Authorize]
    public class StagingController : Controller
    {
        private SounDesign_Web_02.Services.StagingService stagingService;
        private SounDesign_Web_02.Services.ProductService productService = new SounDesign_Web_02.Services.ProductService();

        public StagingController()
        {

            stagingService = new SounDesign_Web_02.Services.StagingService();

        }
        public IActionResult Index()
        {

            Stagings.stagings.Clear();
            List<OrderSummary> orderSummaries = new List<OrderSummary>();

            if (HttpContext.Session.Get("orderSummary") != null)
            {
                //Cast your session variable as required
                orderSummaries = (List<OrderSummary>)(HttpContext.Session.GetObject<List<OrderSummary>>("orderSummary"));
            }
            var danger = "0";
            foreach (OrderSummary os in orderSummaries)
            {
                if (os.action != "Success")
                {
                    danger = "1";
                }
            }
            ViewBag.danger = danger;
            return View(orderSummaries);
        }

        public IActionResult Remove(Staging product)
        {
            if (Stagings.stagings.Count > 0) {
                Stagings.stagings.RemoveAt(Stagings.stagings.Count - 1);
            }
            
            return PartialView("_ProductsList", Stagings.stagings);
        }
        public IActionResult RemoveAll(Staging product)
        {
            Stagings.stagings.Clear();
            return PartialView("_ProductsList", Stagings.stagings);
        }

        public IActionResult _ProductsList(Staging staging)
        {

            Stagings.stagings.Add(staging);
            return PartialView(Stagings.stagings);
        }
        public List<Staging> ToList(DataTable dt)
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
            return products;
        }



        [HttpPost]
        public IActionResult NewProject([FromBody] string projectName)
        {
            var myJsonString = projectName;
            var jo = JObject.Parse(myJsonString);
            var id = jo["projectName"].ToString();
            HttpContext.Session.SetObject("newProjectName", id);
            TempData["id"] = id;
            return Json(new { status = "true", msg = "Redirect now" });
        }
        public IActionResult Create()
        {
            Stagings.stagings.Clear();
            if (HttpContext.Session.Get("newProjectName") != null)
            {
                //Cast your session variable as required
                string newName = (string)(HttpContext.Session.GetObject<string>("newProjectName"));
                List<string> rooms = stagingService.GetStagingRoomNames();
                ViewBag.roomNames = rooms;
                TempData["id"] = newName;
                return View("Create");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult ProjectSelected([FromBody] string p)
        { 
            var myJsonString = p;
            var jo = JObject.Parse(myJsonString);
            var id = jo["projectName"].ToString();

            List<Staging> model = ToList(stagingService.GridLoadProjectStaging(id));
            
            HttpContext.Session.SetObject("stagingProjectModel", model);

            return Json(new { status = "true", msg = "Redirect now" });
        }

        public IActionResult DisplayProject()
        {
            List<Staging> myModel = new List<Staging>();
            if (HttpContext.Session.Get("stagingProjectModel") != null)
            {
                //Cast your session variable as required
                myModel = (List<Staging>)(HttpContext.Session.GetObject<List<Staging>>("stagingProjectModel"));
            }

            //---------------------------------------
            //***Possibly add paging to this action***
            //---------------------------------------

            return View(myModel);
        }

        public IActionResult _DisplayProject(string projectName)
        {
            return PartialView(projectName);
        }
        public IActionResult Read()
        {
            var model = stagingService.GetStagingProjectNames();

            return View(model);
        }
        public IActionResult Update()
        {
            Stagings stagings = stagingService.GetAllStaging();

            return View(Stagings.stagings);
        }

        public IActionResult UpdateProduct()
        {
            return View("Read");
        }
        public IActionResult Delete()
        {
            return View();
        }
        public Product GetProductFromSKU(string sku)
        {
            // controller -> service -> DAO.
            if (productService.ContainsSKU(sku))
            {
                //GETTING EMPTY ROW FROM DATA TABLE
                DataTable dt = productService.GetProductFromSKU(sku);
                if (dt == null)
                {
                    return null;
                }
                DataRow dr = dt.Rows[0];
                Product product = new Product(dr[0].ToString(),
                    dr[1].ToString(), dr[2].ToString(), dr[3].ToString(),
                    (ushort)UInt32.Parse(dr[4].ToString()), dr[5].ToString(),
                    dr[6].ToString());
                return product;
            }
            else
            {
                return null;
            }
        }
        public Product GetStagingFromSerial(string serial)
        {
            // controller -> service -> DAO.
            Product p = productService.GetProductFromSerial(serial);
            return p;
        }

        [HttpPost]
        public IActionResult AddToStaging()
        {
           
            OrderSummaries.orderSummaries.Clear();
            foreach (Staging s in Stagings.stagings)
            {
                ushort inStock = 0;
                ushort requiredByJob = s.quantity;

                OrderSummary orderSummary = new OrderSummary();
                if (stagingService.ContainsPrimaryKey(s.serial, s.sku, s.make, s.model))
                {
                    Product p2 = stagingService.GetPrimaryKey(s.serial, s.sku, s.make, s.model);
                    inStock = p2.quantity;

                    if (requiredByJob > inStock)
                    {
                        //The number to be staged is greater than the number in stock
                        orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room,p2.createdTimeStamp,inStock, requiredByJob, "Success",p2.productUserStamp);

                    }
                    else if (requiredByJob == inStock)
                    {
                        orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success", p2.productUserStamp);
                    }
                    else
                    {
                        
                        orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success", p2.productUserStamp);
                    }

                }
                else
                {

                    //Add to inventory

                    productService.InsertProduct(s.serial, s.sku, s.make, s.model, s.quantity, s.description, "", DateTime.Now,User.Identity.Name.ToString());
                    //Add to staging
                    //stagingDAO.MovePKToStaging(s,s.site);
                    Product p2 = stagingService.GetPrimaryKey(s.serial, s.sku, s.make, s.model);
                    inStock = p2.quantity;
                    orderSummary = new OrderSummary(p2.serial, p2.sku, p2.make, p2.model, p2.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success",p2.productUserStamp);
                }
                OrderSummaries.orderSummaries.Add(orderSummary);
            }
            HttpContext.Session.SetObject("orderSummary", OrderSummaries.orderSummaries);
            return View("Index", OrderSummaries.orderSummaries);
        }

        [HttpPost]
        public IActionResult SetUpdateSession([FromBody] string p)
        {
            Data.DAO.Database _db = new Data.DAO.Database();
            Staging staging = Newtonsoft.Json.JsonConvert.DeserializeObject<Staging>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                HttpContext.Session.SetObject("updateThis", staging);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult Detail()
        {
            Staging updateThis = new Staging();
            if (HttpContext.Session.Get("updateThis") != null)
            {
                //Cast your session variable as required
                updateThis = (Staging)(HttpContext.Session.GetObject<Staging>("updateThis"));
            }



            return View(updateThis);
        }

        [HttpPost]
        public IActionResult TakeStaging([FromBody] string p)
        {
            List<OrderSummary> staging = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderSummary>>(p);

            foreach (OrderSummary os in staging)
            {
                if (stagingService.ContainsPrimaryKey(os.serial, os.sku, os.make, os.model))
                {
                    Product p2 = stagingService.GetPrimaryKey(os.serial, os.sku, os.make, os.model);

                    if (os.required > os.inStock)
                    {
                        //Not enough in stock


                    }
                    else if (os.required == os.inStock)
                    {
                        //Success
                        stagingService.StageOrderSummaryPK(os, DateTime.Now,User.Identity.Name.ToString());

                    }
                    else
                    {
                        //Success
                        // requiredByJob < inStock;
                        ushort leftInStock = (ushort)(os.inStock - os.required);
                        stagingService.SetQuantity(leftInStock, p2.serial, p2.sku, p2.make, p2.model);
                        stagingService.StageOrderSummary(os, DateTime.Now, User.Identity.Name.ToString());
                    }

                }
                else
                {
                    //No Primary KEy

                }
            }

            return Json(new { status = "true", msg = "Redirect now" });
        }

        [HttpPost]
        public IActionResult EditStaging([FromBody] string s)
        {

            var staging = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderSummary>>(s);
            //ViewBag.Staging = staging;
            //ViewData["data"] = staging;
            //return View("EditOrderSummary", staging);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                HttpContext.Session.SetObject("orderSummary", staging);
                return Json(new { status = "true", msg = "Redirect now" });
            }

        }

        public IActionResult EditOrderSummary()
        {

            List<OrderSummary> orderSummaries = new List<OrderSummary>();
            if (HttpContext.Session.Get("orderSummary") != null)
            {
                //Cast your session variable as required
                orderSummaries = (List<OrderSummary>)(HttpContext.Session.GetObject<List<OrderSummary>>("orderSummary"));
            }



            return View(orderSummaries);
        }

        [HttpPost]
        public IActionResult OrderSummaryDetail([FromBody] string s)
        {
            var staging = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderSummary>(s);
            Data.DAO.StagingDAO _db = new Data.DAO.StagingDAO();
            Staging stageDetailObject = new Staging();
            try
            {
                stageDetailObject = new Staging(staging.serial, staging.sku, staging.make, staging.model, staging.required,
                    stagingService.GetStagingDescription(staging.serial, staging.sku, staging.make, staging.model),
                    staging.site, staging.room, staging.createdTime, DateTime.Now,staging.createdUserStamp,User.Identity.Name.ToString());
            }
            catch (Exception ex)
            {
                stageDetailObject = new Staging(staging.serial, staging.sku, staging.make, staging.model, staging.required,
                    "",
                    staging.site, staging.room, staging.createdTime, DateTime.Now, staging.createdUserStamp, User.Identity.Name.ToString());
            }

            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                HttpContext.Session.SetObject("updateThis", stageDetailObject);
                return Json(new { status = "true", msg = "Redirect now" });
            }

        }

        [HttpPost]
        public IActionResult BackToEditSummary([FromBody] string p)
        {
            List<OrderSummary> orderSummaries = new List<OrderSummary>();
            Staging s = Newtonsoft.Json.JsonConvert.DeserializeObject<Staging>(p);
            if (HttpContext.Session.Get("orderSummary") != null)
            {
                //Cast your session variable as required
                orderSummaries = (List<OrderSummary>)(HttpContext.Session.GetObject<List<OrderSummary>>("orderSummary"));
            }

            ushort inStock = 0;
            ushort requiredByJob = s.quantity;
            Data.DAO.Database _db = new Data.DAO.Database();
            OrderSummary orderSummary = new OrderSummary();
            if (stagingService.ContainsPrimaryKey(s.serial, s.sku, s.make, s.model))
            {
                Product p2 = stagingService.GetPrimaryKey(s.serial, s.sku, s.make, s.model);
                inStock = p2.quantity;

                if (requiredByJob > inStock)
                {
                    //The number to be staged is greater than the number in stock
                    orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success", s.productUserStamp);

                }
                else if (requiredByJob == inStock)
                {
                    orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success", s.productUserStamp);
                }
                else
                {
                    orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success", s.productUserStamp);
                }
            }
            else
            {
                //Add to inventory

                productService.InsertProduct(s.serial, s.sku, s.make, s.model, s.quantity, s.description, "", DateTime.Now, User.Identity.Name.ToString());
                //Add to staging
                //stagingDAO.MovePKToStaging(s,s.site);
                Product p2 = stagingService.GetPrimaryKey(s.serial, s.sku, s.make, s.model);
                inStock = p2.quantity;
                orderSummary = new OrderSummary(s.serial, s.sku, s.make, s.model, s.description, s.site, s.room, p2.createdTimeStamp, inStock, requiredByJob, "Success", p2.productUserStamp);
            }
            //OrderSummaries.orderSummaries.Add(orderSummary);
            //Replace order summary
            //Remove order summary with pk from order summaries
            foreach (OrderSummary os in orderSummaries)
            {
                if (os.serial == orderSummary.serial && os.sku == orderSummary.sku && os.make == orderSummary.make && os.model == orderSummary.model)
                {
                    orderSummaries.Remove(os);
                    break;
                }
            }
            //add order summary to order summaries
            orderSummaries.Add(orderSummary);
            //set session object and redirect
            HttpContext.Session.SetObject("orderSummary", orderSummaries);
            return Json(new { status = "true", msg = "Redirect now" });
        }

        [HttpPost]
        public IActionResult DeleteOrderSummaryItem([FromBody] string p)
        {
            List<OrderSummary> orderSummaries = new List<OrderSummary>();
            Staging s = Newtonsoft.Json.JsonConvert.DeserializeObject<Staging>(p);
            if (HttpContext.Session.Get("orderSummary") != null)
            {
                //Cast your session variable as required
                orderSummaries = (List<OrderSummary>)(HttpContext.Session.GetObject<List<OrderSummary>>("orderSummary"));
            }


            var remove = orderSummaries.Where(os => os.serial == s.serial && os.sku == s.sku
            && os.make == s.make && os.model == s.model && os.room == s.room).First();
            orderSummaries.Remove(remove);
            HttpContext.Session.SetObject("orderSummary", orderSummaries);
            return Json(new { status = "true", msg = "Redirect now" });
        }

        [HttpPost]
        public IActionResult SetUpdateSessionDetailPage([FromBody] string p)
        {
            Staging product = Newtonsoft.Json.JsonConvert.DeserializeObject<Staging>(p);
            product = stagingService.GetStagingItem(product.serial, product.sku, product.make,
                product.model, product.site, product.room);
            if (product is null || !ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Product not valid." });
            }
            else
            {

                HttpContext.Session.SetObject("updateThisDetailPage", product);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult DetailUpdatePage()
        {
            Staging updateThis = new Staging();
            if (HttpContext.Session.Get("updateThisDetailPage") != null)
            {
                //Cast your session variable as required
                updateThis = (Staging)(HttpContext.Session.GetObject<Staging>("updateThisDetailPage"));
            }


            return View(updateThis);
        }

        [HttpPost]
        public IActionResult SetDeleteSessionStaging([FromBody] string p)
        {
            Staging product = Newtonsoft.Json.JsonConvert.DeserializeObject<Staging>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                HttpContext.Session.SetObject("deleteThisStaging", product);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult DetailDeleteStaging()
        {
            Staging updateThis = new Staging();
            if (HttpContext.Session.Get("deleteThisStaging") != null)
            {
                //Cast your session variable as required
                updateThis = (Staging)(HttpContext.Session.GetObject<Staging>("deleteThisStaging"));
            }



            return View(updateThis);
        }

        [HttpPost]
        public IActionResult FinalDeleteStaging([FromBody] string p)
        {
            Staging product = Newtonsoft.Json.JsonConvert.DeserializeObject<Staging>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {
                stagingService.DeleteProductStaging(product.serial,product.sku,product.make,product.model,
                    product.site,product.room);

                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        [HttpPost]
        public IActionResult MoveStagingToInventory([FromBody] string product) {

            Product p = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(product);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                int incomingStatus = productService.IncomingProductStatus(p.serial, p.sku, p.make, p.model);

                switch (incomingStatus)
                {
                    case 0:
                        {
                            //Primary key exists in database
                            //Update quantity
                            if (User.Identity.Name.ToString() is not null)
                            {
                                productService.UpdateQuantity_PK_Exists(p.serial, p.sku, p.make,
                                    p.model, p.quantity, DateTime.Now, User.Identity.Name.ToString());
                            }

                            break;
                        }
                    case 1:
                        {
                            //Primary key does not exist in database, but make and model exist
                            //Prompt user to create new product or update quantity
                            if (User.Identity.Name.ToString() is not null)
                            {
                                productService.InsertProduct(p.serial, p.sku, p.make, p.model, p.quantity,
                                    p.description, p.location, DateTime.Now, User.Identity.Name.ToString());
                            }

                            break;
                        }
                    case 2:
                        {
                            //Neither primary key or make and model exist
                            //Insert product
                            if (User.Identity.Name.ToString() is not null)
                            {
                                productService.InsertProduct(p.serial, p.sku, p.make, p.model, p.quantity,
                                    p.description, p.location, DateTime.Now, User.Identity.Name.ToString());
                            }
                            else
                            {

                            }
                            break;
                        }
                    default:
                        {
                            //Some error has occured. Do not insert product

                            break;
                        }
                }
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

    }


}
