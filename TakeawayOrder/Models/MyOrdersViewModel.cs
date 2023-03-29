using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class MyOrdersViewModel
    {
        public Order Order { get; set; }
        public decimal OrderTotal { get; set; } = 0;
        public decimal OrderTotalDiscounted { get; set; } = 0;
        public List<Order> MyOrders { get; set; }
        public List<ProductOrderViewModel> ProductOrders { get; set; }
    }
}
