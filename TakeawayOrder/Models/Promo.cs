using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class Promo
    {
        public long Id { get; set; }
        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
    }
}
