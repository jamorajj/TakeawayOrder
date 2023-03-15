using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TakeawayOrder.Data;
using TakeawayOrder.Models;

namespace TakeawayOrder.Controllers
{
    public class OrderController : Controller
    {
        private DataContext _context;

        public OrderController(DataContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Kitchen,Admin,Cashier")]
        public IActionResult Index()
        {
            return View(_context.Orders.ToList());
        }
        [Authorize(Roles = "Kitchen,Admin,Cashier")]
        public async Task<IActionResult> Edit(long id)
        {
            Order order = await _context.Orders.FindAsync(id);
            var productOrders = await _context.ProductOrder.Where(x => x.OrderId == id).ToListAsync();
            OrderViewModel orderVM = new OrderViewModel();

            orderVM.OrderId = order.Id;
            orderVM.OrderName = order.OrderName;
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
