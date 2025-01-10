using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SounDesign_Web_02.Data;
using SounDesign_Web_02.Models;

namespace SounDesign_Web_02.Controllers
{
    [Authorize]
    public class CheckInventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

        public CheckInventoryController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            this.db = db;

        }
        public List<CheckInventory> OnPostUploadAsync(List<IFormFile> files)
        {
          
            if (files[0]==null)
            {
                return new List<CheckInventory>();
            }
            long size = files.Sum(f => f.Length);
            List<CheckInventory> po = new List<CheckInventory>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    var filePath = Path.GetTempPath();
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    //var filePath = @"C:\Users\Chris\Source\Repos\SounDesign_Web_01\SounDesign_Web_01\wwwroot\files\C:\Users\Chris\Source\Repos\SounDesign_Web_01\SounDesign_Web_01\wwwroot\files\";
                    using (var stream = System.IO.File.Create(filePath + "/" + formFile.FileName))
                    {
                        formFile.CopyToAsync(stream);
                        stream.Close();
                    }

                    string nameOfFile = filePath + "/" + formFile.FileName;

                    Excel excel = new Excel();

                    DataSet ds = excel.ReadCSV(nameOfFile);
                    DataTable dt = ds.Tables[0];
                    Dictionary<string, int> required = new Dictionary<string, int>();


                    foreach (DataRow dr in dt.Rows)
                    { // map
                        string currentMake = "";
                        string currentModel = "";
                        int numberRequired = 0;
                        if (dt.Columns.Count == 23 || dt.Columns.Count == 26)
                        {
                            currentMake = dr[1].ToString();
                            currentModel = dr[2].ToString();
                            if (currentMake == "Brand" && currentModel == "Model")
                            {
                                continue;
                            }
                            numberRequired = 0;
                            Int32.TryParse(dr[7].ToString(), out numberRequired);
                        }
                        else if (dt.Columns.Count == 22)
                        {
                            currentMake = dr[2].ToString();
                            currentModel = dr[3].ToString();
                            if ((currentMake == "Brand" && currentModel == "ModelNumber")
                                || (currentMake == "" && currentModel == ""))
                            {
                                continue;
                            }
                            numberRequired = 0;
                            var a = ((int)Convert.ToDouble(dr[5].ToString())).ToString();
                            Int32.TryParse(a, out numberRequired);
                        }
                        else
                        {
                            currentMake = "";
                            currentModel = "";
                            numberRequired = 0;
                        }

                        string key = currentMake + "~" + currentModel;

                        if (required.ContainsKey(key))
                        {
                            int val = required[key];
                            required[key] = val + numberRequired;
                        }
                        else
                        {
                            required.Add(key, numberRequired);
                        }
                    }


                    foreach (string key in required.Keys)
                    {
                        /*
                    if key in map
                        add request to existing map row
                    else
                        add key and request to map

                     * 
                     */

                        string[] keys = key.Split("~"); //   currentMake + "~" + currentModel;
                        string currentMake = keys[0];
                        string currentModel = keys[1];
                        int numberRequired = required[key];


                        Data.DAO.Database database = new Data.DAO.Database();
                        if (database.ContainsMakeModel(currentMake, currentModel))
                        {
                            //Find number of make and models that match
                            //Add quantities of each
                            int stock = database.NumberOfMatchingMakeModel(currentMake, currentModel);
                            //Determine whether enough is in stock or more are required

                            int order = numberRequired - stock;
                            if (order < 0)
                            {
                                order = 0;
                            }
                            CheckInventory cI = new CheckInventory(currentMake, currentModel, numberRequired, stock, order);
                            po.Add(cI);
                        }
                        else
                        {
                            //Not in stock

                            po.Add(new CheckInventory(currentMake, currentModel, numberRequired, 0, numberRequired));
                        }
                    }

                }
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return po;
        }
        

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file1)
        {
            List<IFormFile> files = new List<IFormFile>();
            files.Add(file1);
            List<CheckInventory> po = OnPostUploadAsync(files);

            return View("PO", po);
        }
    }
}
