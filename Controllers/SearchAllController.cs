using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SounDesign_Web_02.Data;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Controllers
{
    [Authorize]
    public class SearchAllController : Controller
    {
		private readonly ApplicationDbContext db;

		public SearchAllController(ILogger<HomeController> logger, ApplicationDbContext db)
		{
			this.db = db;

		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Search([FromBody] string searchString)
		{
			List<SounDesignSearchProduct> searchResults = new List<SounDesignSearchProduct>();
			string[] arrayOfStrings = searchString.Split(' ');
			var products = from s in db.Inventory
						   select s;
			var staged = from s in db.Staging
						 select s;
			var trucked = from s in db.Truck
						  select s;




			if (!String.IsNullOrEmpty(searchString))
			{
				foreach (string st in arrayOfStrings)
				{

					products = products.Where(p => p.make.ToLower().Contains(st.ToLower())
							|| p.model.ToLower().Contains(st.ToLower()) ||
							p.description.ToLower().Contains(st.ToLower()));

					staged = staged.Where(p => p.make.ToLower().Contains(st.ToLower())
						|| p.model.ToLower().Contains(st.ToLower()) ||
						p.description.ToLower().Contains(st.ToLower()));

					trucked = trucked.Where(p => p.make.ToLower().Contains(st.ToLower())
						|| p.model.ToLower().Contains(st.ToLower()) ||
						p.description.ToLower().Contains(st.ToLower()));
				}

			}

			foreach (Product p in products)
			{
				searchResults.Add(new SounDesignSearchProduct(
					p.ID, p.serial, p.sku, p.make, p.model, p.quantity, p.description, p.location));
			}

			foreach (Staging s in staged)
			{
				searchResults.Add(new SounDesignSearchProduct(s.Id, s.serial, s.sku, s.make, s.model, s.quantity, s.description, s.site, s.room));
			}

			foreach (Truck t in trucked)
			{
				searchResults.Add(new SounDesignSearchProduct(t.Id, t.serial, t.sku, t.make, t.model, t.quantity, t.description, t.site, t.room, t.technician, t.signature));
			}




			HttpContext.Session.SetObject("searchResults", searchResults);


			return Json(new { status = "true", msg = "Redirect now" });
		}

		public IActionResult Search1()
		{
			List<SounDesignSearchProduct> mySearchResults = new List<SounDesignSearchProduct>();
			if (HttpContext.Session.Get("searchResults") != null)
			{
				//Cast your session variable as required
				mySearchResults = (List<SounDesignSearchProduct>)(HttpContext.Session.GetObject<List<SounDesignSearchProduct>>("searchResults"));
			}
			return View("Index", mySearchResults);
		}
	}
}
