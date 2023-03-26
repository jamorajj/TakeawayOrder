using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class MyOrdersViewModel
    {
        public Order Order { get; set; }
        public List<Order> MyOrders { get; set; }
        public List<ProductOrderViewModel> ProductOrders { get; set; }
    }
}
