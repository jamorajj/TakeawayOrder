using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TakeawayOrder.Data;
using TakeawayOrder.Models;

namespace TakeawayOrder.Controllers
{
    public class OrderController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private DataContext _context;

        public OrderController(UserManager<ApplicationUser> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "Staff,Admin")]
        public IActionResult Index()
        {
            return View(_context.Orders.ToList());
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyOrders()
        {
            MyOrdersViewModel myOrdersVM = new MyOrdersViewModel();

            // get an ongoing order, if there is any
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing);
            // get all the orders of the current user
            var myOrders = _context.Orders.Where(x => x.UserName == User.Identity.Name).ToList();
            // if there is an ongoing order...
            if (order != null)
            {
                // get all the product under that ongoing order...
                var prodOrders = _context.ProductOrder.Where(x => x.OrderId == order.Id).ToList();

                myOrdersVM.ProductOrders = new List<ProductOrderViewModel>();
                foreach (var prodOrder in prodOrders)
                {
                    Product p = await _context.Products.FindAsync(prodOrder.ProductId);

                    ProductOrderViewModel prodOrderVM = new ProductOrderViewModel();
                    prodOrderVM.ProductId = p.Id;
                    prodOrderVM.ProductName = p.Name;
                    prodOrderVM.ProductPrice = p.Price;
                    prodOrderVM.ProductQuantity = prodOrder.Quantity;

                    myOrdersVM.ProductOrders.Add(prodOrderVM);
                }
            }

            myOrdersVM.Order = order;
            myOrdersVM.MyOrders = myOrders;

            return View(myOrdersVM);
        }
        [Authorize(Roles = "Staff,Customer,Admin")]
        public async Task<IActionResult> DeductOne(string id)
        {
            if (User.Identity.Name == null)
            {
                return Redirect("/Account/Login");
            }

            long lId = long.Parse(id);

            var product = _context.Products.FirstOrDefault(x => x.Id == lId);
            TempData["flash"] = $"Deducted 1 {product.Name} to your order.";

            // try to get an ongoing order...
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing);
            var prodOrder = _context.ProductOrder.FirstOrDefault(x => x.ProductId == lId && x.OrderId == order.Id);

            prodOrder.Quantity = prodOrder.Quantity - 1;

            if (prodOrder.Quantity == 0)
            {
                _context.ProductOrder.Remove(prodOrder);
            } else
            {
                _context.ProductOrder.Update(prodOrder);
            }
            
            await _context.SaveChangesAsync();

            return Redirect("/Order/MyOrders");
        }
        [Authorize(Roles = "Staff,Customer,Admin")]
        public async Task<IActionResult> AddOne(string id)
        {
            if (User.Identity.Name == null)
            {
                return Redirect("/Account/Login");
            }

            long lId = long.Parse(id);

            var product = _context.Products.FirstOrDefault(x => x.Id == lId);
            TempData["flash"] = $"Added 1 {product.Name} to your order.";

            // try to get an ongoing order...
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing);
            var prodOrder = _context.ProductOrder.FirstOrDefault(x => x.ProductId == lId && x.OrderId == order.Id);

            prodOrder.Quantity = prodOrder.Quantity + 1;
            _context.ProductOrder.Update(prodOrder);
            await _context.SaveChangesAsync();

            return Redirect("/Order/MyOrders");
        }
        [Authorize(Roles = "Staff,Customer,Admin")]
        public async Task<IActionResult> RemoveToCart(string id)
        {
            if (User.Identity.Name == null)
            {
                return Redirect("/Account/Login");
            }

            long lId = long.Parse(id);

            var product = _context.Products.FirstOrDefault(x => x.Id == lId);
            TempData["flash"] = $"Removed {product.Name} to your order.";

            // try to get an ongoing order...
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing);
            var prodOrder = _context.ProductOrder.FirstOrDefault(x => x.ProductId == lId && x.OrderId == order.Id);

            _context.ProductOrder.Remove(prodOrder);
            await _context.SaveChangesAsync();

            return Redirect("/Order/MyOrders");
        }
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Checkout(long id)
        {
            TempData["flash"] = $"Your order has been checked out. Please come to the shop to pay and claim your order. Thank you for shopping at Takeaway10!";

            // try to get an ongoing order...
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing && x.Id == id);
            order.status = OrderStatus.Pickup;

            _context.Orders.Update(order);

            ApplicationUser userToEdit = await _userManager.FindByNameAsync(User.Identity.Name);
            userToEdit.CheckoutCount = userToEdit.CheckoutCount + 1;

            await _userManager.UpdateAsync(userToEdit);
            await _context.SaveChangesAsync();

            return Redirect("/Order/MyOrders");
        }
        // TODO: delete this?
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Edit(long id)
        {
            Order order = await _context.Orders.FindAsync(id);
            var productOrders = await _context.ProductOrder.Where(x => x.OrderId == id).ToListAsync();
            OrderViewModel orderVM = new OrderViewModel();

            orderVM.OrderId = order.Id;
            orderVM.OrderStatus = order.status;
            orderVM.OrderProducts = new List<Product>();

            foreach (var productInOrder in productOrders)
            {
                Product p = await _context.Products.FindAsync(productInOrder.ProductId);

                orderVM.OrderProducts.Add(p);
            }

            return View(orderVM);
        }   
        [HttpPost]
        public async Task<IActionResult> Edit(OrderViewModel orderVM)
        {
            Order order = await _context.Orders.FindAsync(orderVM.OrderId);

            order.status = orderVM.OrderStatus;

            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
