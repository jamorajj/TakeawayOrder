namespace TakeawayOrder.Models
{
    public class Order
    {
        public long Id { get; set; }
        public string OrderName { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public OrderStatus status { get; set; }
    }

    public enum OrderStatus
    {
        Placed,
        Paid,
        Cancel,
        Pickup,
        Done
    }
}
