using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TakeawayOrder.Models
{
    public class Product
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, Range(0.01, double.MaxValue)]
        [Column(TypeName = "decimal(8,2)")]
        public decimal Price { get; set; }
    }
}
