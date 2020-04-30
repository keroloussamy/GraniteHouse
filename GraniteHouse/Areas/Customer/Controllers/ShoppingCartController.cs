using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.ExtentionMethods;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")]
    public class ShoppingCartController : Controller
    {

        private readonly ApplicationDbContext _db;
        [BindProperty]
        public ShppingCartViewModel ShoppingCartVM { get; set; }
        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShppingCartViewModel()
            {
                //Appointments = new Models.Appointments(),
                Products = new List<Models.Products>()
            };
        }

        

        public IActionResult Index()
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if (listShoppingCart != null)
            {
                if (listShoppingCart.Count > 0)
                {
                    foreach (var item in listShoppingCart)
                    {
                        Products pro = _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags).Where(m => m.Id == item).FirstOrDefault();
                        ShoppingCartVM.Products.Add(pro);
                    }
                }
            }
            return View(ShoppingCartVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            ShoppingCartVM.Appointments.AppointmentDate = ShoppingCartVM.Appointments.AppointmentDate
                                                          .AddHours(ShoppingCartVM.Appointments.AppointmentTime.Hour)
                                                          .AddMinutes(ShoppingCartVM.Appointments.AppointmentTime.Minute);

            Appointments appointments = ShoppingCartVM.Appointments;
            _db.Appointments.Add(appointments);
            _db.SaveChanges();

            int appointmentId = appointments.Id;
            foreach (int item in listShoppingCart)
            {
                ProductsSelectedForAppointment productsSelectedForAppointment = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = item
                };
                _db.SelectedForAppointments.Add(productsSelectedForAppointment);
            }
            _db.SaveChanges();
            listShoppingCart = new List<int>();
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction("Confirmation", "ShoppingCart", new { id = appointmentId});

        }


        public IActionResult Remove(int id)
        {
            List<int> listShoppingCart = HttpContext.Session.Get<List<int>>("ssShoppingCart");
            if(listShoppingCart.Count > 0)
            {
                if (listShoppingCart.Contains(id))
                {
                    listShoppingCart.Remove(id);
                }
            }
            HttpContext.Session.Set("ssShoppingCart", listShoppingCart);

            return RedirectToAction("Index");
        }



        public IActionResult Confirmation(int Id)
        {
            ShoppingCartVM.Appointments = _db.Appointments.Where(a => a.Id == Id).FirstOrDefault();
            List<ProductsSelectedForAppointment> ListproductsSelectedForAppointment = 
                _db.SelectedForAppointments.Where(m => m.AppointmentId == Id).ToList();


            foreach (var item in ListproductsSelectedForAppointment)
            {
                ShoppingCartVM.Products.Add(_db.Products.Include(p => p.ProductTypes).Include(p => p.SpecialTags)
                    .Where(p => p.Id == item.ProductId).FirstOrDefault());
            }


            return View(ShoppingCartVM);
        }



    }
}