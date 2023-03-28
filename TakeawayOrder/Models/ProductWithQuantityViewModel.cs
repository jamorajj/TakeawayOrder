namespace TakeawayOrder.Models
{
    public class ProductWithQuantityViewModel
    {
        public long ProductId { get; set;}
        public string ProductName { get; set;}
        public decimal ProductPrice { get; set;}
        public int ProductQuantity { get; set; }
        public decimal ProductTotal{ get; set; }
    }
}
