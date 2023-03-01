using Microsoft.EntityFrameworkCore;
using TakeawayOrder.Models;

namespace TakeawayOrder.Data
{
    public class SeedData
    {
        public static void SeedDatabase(DataContext context)
        {
            context.Database.Migrate();

            if (context.Products.Count() == 0)
            {
                context.Products.AddRange(
                    new Product
                    {
                        Name = "Apple Pie Whole",
                        Price = 8.50M
                    },
                    new Product
                    {
                        Name = "Coffee",
                        Price = 2.00M
                    },
                    new Product
                    {
                        Name = "Chocolate Cake Slice",
                        Price = 4.50M
                    },
                    new Product
                    {
                        Name = "Orange Juice",
                        Price = 1.80M
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
