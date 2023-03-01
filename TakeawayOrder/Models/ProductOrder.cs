namespace TakeawayOrder.Models
{
    public class ProductOrder
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long OrderId { get; set; }
    }
}
