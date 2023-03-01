using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TakeawayOrder.Data;
using TakeawayOrder.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TakeawayOrder.Controllers
{
    public class HomeController : Controller
    {
        private DataContext _context;

        public HomeController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Orders.ToList());
        }
        public IActionResult NewOrder()
        {
            List<ProductOnOrderViewModel> productList = new List<ProductOnOrderViewModel>();

            var products = _context.Products.ToList();

            // users view model to get roles per user
            foreach (var product in products)
            {
                ProductOnOrderViewModel productOnOrder = new ProductOnOrderViewModel();

                productOnOrder.ProductId = product.Id;
                productOnOrder.ProductName = product.Name;
                productOnOrder.ProductPrice = product.Price;
                productOnOrder.IsChecked = false;

                productList.Add(productOnOrder);
            }

            return View(productList);
        }
        [HttpPost]
        public async Task<IActionResult> NewOrder(List<ProductOnOrderViewModel> productOnOrderVM)
        {
            var selectedProducts = productOnOrderVM.Where(x => x.IsChecked).ToList();

            if (selectedProducts.Count() > 0)
            {
                var totalOrders = _context.Orders.ToList().Count();

                var order = new Order();
                order.OrderName = totalOrders.ToString();
                order.status = OrderStatus.Placed;
                _context.Add(order);
                await _context.SaveChangesAsync();

                foreach (var product in selectedProducts)
                {
                    var po = new ProductOrder();
                    po.OrderId = order.Id;
                    po.ProductId= product.ProductId;

                    _context.Add(po);
                    await _context.SaveChangesAsync();
                }
                TempData["flash"] = order.OrderName;
                return RedirectToAction("OrderInfo");
            }

            ModelState.AddModelError("", "Please select a product");

            return View(productOnOrderVM);
        }
        public IActionResult OrderInfo()
        {
            return View();
        }
    }
}