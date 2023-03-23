using Microsoft.AspNetCore.Mvc;
using TakeawayOrder.Data;
using TakeawayOrder.Models;

namespace TakeawayOrder.ViewComponents
{
    [ViewComponent(Name = "OrderCount")]
    public class OrderCountComponent : ViewComponent
    {
        private DataContext _context;

        public OrderCountComponent(DataContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var order = _context.Orders.FirstOrDefault(x => x.UserName == User.Identity.Name && x.status == OrderStatus.Ongoing);
            int orderCount = 0;
            if (order != null)
            {
                var prodOrders = _context.ProductOrder.Where(x => x.OrderId == order.Id).ToList();

                if (prodOrders != null)
                {
                    foreach (var prodOrder in prodOrders)
                    {
                        orderCount += prodOrder.Quantity;
                    }
                }
            }
            ViewBag.OrderCount = orderCount;
            return View("Index");
        }
    }
}
