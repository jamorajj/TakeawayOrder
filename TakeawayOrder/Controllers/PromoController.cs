using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TakeawayOrder.Data;
using TakeawayOrder.Models;

namespace TakeawayOrder.Controllers
{
    public class PromoController : Controller
    {
        private DataContext _context;

        public PromoController(DataContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult Index()
        {
            // check if active promo....
            Promo p = _context.Promos.Where(x => x.EndDate > DateTime.Today).FirstOrDefault();

            // viewbag if promo is still active...
            ViewBag.hasPromo = false;
            if (p != null)
            {
                ViewBag.hasPromo = true;
                ViewBag.endDate = p.EndDate;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Promo promo)
        {
            if (promo.EndDate > DateTime.Today)
            {
                _context.Add(promo);
                await _context.SaveChangesAsync();
            }
            else
            {
                ViewBag.hasPromo = false;

                Console.WriteLine("Less Than");
                ModelState.AddModelError("", "Promo date must be later than current date.");
                return View(promo);
            }

            return Redirect("/Promo");
        }
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Deactivate()
        {
            // check if active promo....
            Promo p = _context.Promos.Where(x => x.EndDate > DateTime.Today).FirstOrDefault();

            if (p != null)
            {
                _context.Promos.Remove(p);
            }

            await _context.SaveChangesAsync();

            return Redirect("/Promo");
        }
    }
}
