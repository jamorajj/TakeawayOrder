namespace TakeawayOrder.Models
{
    public class Order
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public OrderStatus status { get; set; }
        public bool IsPromo { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public enum OrderStatus
    {
        Ongoing,
        Pickup,
        Done
    }
}
