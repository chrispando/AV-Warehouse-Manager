using Microsoft.AspNetCore.Mvc;
using SounDesign_Web_02.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;

namespace SounDesign_Web_02.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private SounDesign_Web_02.Services.ProductService productService = new SounDesign_Web_02.Services.ProductService();
        public IActionResult Index()
        {
            Products.products.Clear();
            return View();
        }

        public IActionResult Remove(Product product)
        {
            Products products = new Products();
            if (Products.products.Count > 0) {
                Products.products.RemoveAt(Products.products.Count - 1);
            }
            
            return PartialView("_ProductsList", Products.products);
        }
        public IActionResult RemoveAll(Product product)
        {
            Products products = new Products();
            Products.products.Clear();
            return PartialView("_ProductsList", Products.products);
        }
        public IActionResult _ProductsList(Product product)
        {

            Products.products.Add(product);
            return PartialView(Products.products);
        }
        public IActionResult Create()
        {
            Products.products.Clear();

            return View();
        }
        [HttpPost]
        public IActionResult AddToInventory()
        {
            // controller -> service -> DAO.
            foreach (Product p in Products.products)
            {
                int incomingStatus = productService.IncomingProductStatus(p.serial,p.sku, p.make, p.model);

                switch (incomingStatus)
                {
                    case 0:
                        {
                            //Primary key exists in database
                            //Update quantity
                            if (User.Identity.Name.ToString() is not null) {
                                productService.UpdateQuantity_PK_Exists(p.serial, p.sku, p.make,
                                    p.model, p.quantity, DateTime.Now, User.Identity.Name.ToString());
                            }
                            
                            break;
                        }
                    case 1:
                        {
                            //Primary key does not exist in database, but make and model exist
                            //Prompt user to create new product or update quantity
                            if (User.Identity.Name.ToString() is not null) {
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
                            else { 
                                
                            }
                            break;
                        }
                    default:
                        {
                            //Some error has occured. Do not insert product

                            break;
                        }
                }
            }


            return View("Index");
        }

        public async Task<IActionResult> Read(string sortOrder, string searchString, int pg = 1, int pageSize = 6)
        {
            Products.products.Clear();
            ViewData["MakeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "make_desc" : "";
            ViewData["ModelSortParm"] = sortOrder == "Model" ? "model_desc" : "Model";
            ViewData["CurrentFilter"] = searchString;

            // controller -> service -> DAO.
            Products products2 = productService.GetAllInventory();
            List<Product> products3 = Products.products;
            var products = products3.AsQueryable<Product>();
            products = products.Where(p=>p.quantity > 0);

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.make.ToLower().Contains(searchString.ToLower())
                    || p.model.ToLower().Contains(searchString.ToLower()));
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
            return View(data);
        }

        public IActionResult Update()
        {
            Products products = productService.GetAllInventory();
            return View(Products.products);
        }

        [HttpPost]
        public IActionResult UpdateProduct([FromBody] string p)
        {
            Product product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(p);
            productService.UpdateProduct(product);
            return Json(new { status = "true", msg = "Redirect now" });
        }

        [HttpPost]
        public IActionResult SetUpdateSession([FromBody] string p)
        {
            Product product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(p);
            product = productService.GetProductToUpdate(product);
            if (product is null || !ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Product not valid." });
            }
            else
            {

                HttpContext.Session.SetObject("updateThis", product);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult Detail()
        {
            Product updateThis = new Product();
            if (HttpContext.Session.Get("updateThis") != null)
            {
                //Cast your session variable as required
                updateThis = (Product)(HttpContext.Session.GetObject<Product>("updateThis"));
            }


            return View(updateThis);
        }

        public IActionResult Delete()
        {
            Products.products.Clear();
            Products products = productService.GetAllInventory();

            return View(Products.products);
        }

        [HttpPost]
        public IActionResult SetDeleteSession([FromBody] string p)
        {
            Product product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {

                HttpContext.Session.SetObject("deleteThis", product);
                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public IActionResult DetailDelete()
        {
            Product updateThis = new Product();
            if (HttpContext.Session.Get("deleteThis") != null)
            {
                //Cast your session variable as required
                updateThis = (Product)(HttpContext.Session.GetObject<Product>("deleteThis"));
            }



            return View(updateThis);
        }

        [HttpPost]
        public IActionResult FinalDelete([FromBody] string p)
        {
            Product product = Newtonsoft.Json.JsonConvert.DeserializeObject<Product>(p);
            if (!ModelState.IsValid)
            {
                return Json(new { status = "false", msg = "Enter required fields." });
            }
            else
            {
                productService.DeleteProduct(product);

                return Json(new { status = "true", msg = "Redirect now" });
            }
        }

        public Product GetProductFromSKU(string sku)
        {

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
        public Product GetProductFromSerial(string serial)
        {  // controller -> service -> DAO.
            Product product = productService.GetProductFromSerial(serial);
            return product;
        }

        [HttpPost]
        public List<string> GetModelFromMake([FromBody] string make)
        {
            List<string> models = new List<string>();
            var myJsonString = make;
            var jo = JObject.Parse(myJsonString);
            var id = jo["make"].ToString();
            models = productService.GetModelsFromMake(id);
            return models;
        }

        public List<string> GetListOfBrands() {
            return productService.GetListOfBrands();
        }
    }
}
