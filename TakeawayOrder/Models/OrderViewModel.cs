using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class OrderViewModel
    {
        public long OrderId { get; set; }
        public string OrderName { get; set; }
        public List<Product> OrderProducts { get; set; }
        [Display(Name = "Order Status")]
        public OrderStatus OrderStatus { get; set; }
    }
}
