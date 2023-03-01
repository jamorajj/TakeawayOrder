using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TakeawayOrder.Data;

namespace TakeawayOrder.Controllers
{
    public class ProductController : Controller
    {
        private DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }
    }
}
