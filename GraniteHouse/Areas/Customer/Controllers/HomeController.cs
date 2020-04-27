using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GraniteHouse.Models;
using GraniteHouse.Data;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.ExtentionMethods;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    //[Route("Customer/[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var ProductList = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).ToListAsync();
            return View(ProductList);
        }

        [Route("Customer/[controller]/[action]")]
        public async Task<IActionResult> Details(int id)
        {
            var Product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m=>m.Id == id).FirstOrDefaultAsync();
            return View(Product);
        }


        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        [Route("Customer/[controller]/[action]")]
        public IActionResult DetailsPost(int id)
        {
            List<int> lstShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (lstShoppingCart == null)
            {
                lstShoppingCart = new List<int>();
            }
            lstShoppingCart.Add(id);
            HttpContext.Session.Set("ssShoppingCart", lstShoppingCart);

            return RedirectToAction("Index");

        }

        [Route("Customer/[controller]/[action]")]
        public IActionResult Remove(int id)
        {
            List<int> ListShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(ListShoppingCart.Count > 0)
            {
                if (ListShoppingCart.Contains(id))
                {
                    ListShoppingCart.Remove(id);
                }
            }

            HttpContext.Session.Set("ssShoppingCart", ListShoppingCart);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
