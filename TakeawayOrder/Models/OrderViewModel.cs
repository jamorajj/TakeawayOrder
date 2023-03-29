using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class OrderViewModel
    {
        public long OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderName { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal DiscountedOrderTotal { get; set; } = 0;
        public bool IsPromo { get; set; }
        public List<ProductWithQuantityViewModel> OrderProducts { get; set; }
        [Display(Name = "Order Status")]
        public OrderStatus OrderStatus { get; set; }
    }
}
