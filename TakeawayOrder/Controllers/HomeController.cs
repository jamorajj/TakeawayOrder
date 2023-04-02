using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TakeawayOrder.Data;
using TakeawayOrder.Models;

namespace TakeawayOrder.Controllers
{
    public class HomeController : Controller
    {
        private DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }
        [Route("Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            return View(new ErrorViewModel { StatusCode = statusCode, OriginalPath = feature?.OriginalPath });
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var products = from m in _context.Products
                         select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name!.Contains(searchString));
            }

            ViewBag.Role = "Customer";

            if (User.IsInRole("Admin"))
            {
                ViewBag.Role = "Admin";
            }
            if (User.IsInRole("Staff"))
            {
                ViewBag.Role = "Staff";
            }

            ViewBag.hasSearchString = String.IsNullOrEmpty(searchString);

            // check if active promo....
            Promo p = _context.Promos.Where(x => x.EndDate > DateTime.Today).FirstOrDefault();

            // viewbag if promo is still active...
            ViewBag.hasPromo = false;
            if (p != null)
            {
                ViewBag.hasPromo = true;
                ViewBag.endDate = p.EndDate;
            }

            return View(await products.ToListAsync());
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> AddToOrder(string id, string quantity)
        {
            if (User.Identity.Name == null)
            {
                return Redirect("/Account/Login");
            }

            long lId = long.Parse(id);
            int iQuantity = int.Parse(quantity);

            var product = _context.Products.FirstOrDefault(x => x.Id == lId);
            TempData["flash"] = $"Added {quantity} {product.Name} to your order.";

            // try to get an ongoing order...
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing);

            // if no order yet, create a new one...
            if (order == null)
            {
                var newOrder = new Order();
                newOrder.status = OrderStatus.Ongoing;
                newOrder.UserName = User.Identity.Name;
                newOrder.IsPromo = false;
                newOrder.OrderDate = DateTime.Now;
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                var newProdOrder = new ProductOrder();
                newProdOrder.Quantity = iQuantity;
                newProdOrder.ProductId = lId;
                newProdOrder.OrderId = newOrder.Id;

                _context.ProductOrder.Add(newProdOrder);
                await _context.SaveChangesAsync();
            } else
            {
                // else, get the product from order
                var prodOrder = _context.ProductOrder.FirstOrDefault(x => x.ProductId == lId && x.OrderId == order.Id);
                
                // if no product for this order, add new productOrder...
                if (prodOrder == null)
                {
                    var newProdOrder = new ProductOrder();
                    newProdOrder.Quantity = iQuantity;
                    newProdOrder.ProductId = lId;
                    newProdOrder.OrderId = order.Id;

                    _context.ProductOrder.Add(newProdOrder);
                    await _context.SaveChangesAsync();
                } else
                {
                    // else, add the quantity of existing productOrder
                    prodOrder.Quantity = prodOrder.Quantity + iQuantity;
                    _context.ProductOrder.Update(prodOrder);
                    await _context.SaveChangesAsync();
                }
            }

            return Redirect("/");
        }
    }
}