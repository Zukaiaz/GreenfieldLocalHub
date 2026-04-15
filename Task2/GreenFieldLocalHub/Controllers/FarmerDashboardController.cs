using GreenFieldLocalHub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GreenFieldLocalHub.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class FarmerDashboardController : Controller
    {

        public readonly ApplicationDbContext _context;

        public FarmerDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var farmers = await _context.Farmers
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (farmers == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Where(x => x.FarmersId == farmers.FarmersId)
                .ToListAsync();

            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Products)
                .Where(o => o.OrderProducts
                    .Any(op => op.Products.FarmersId == farmers.FarmersId))
                .ToListAsync();

            ViewBag.TotalProducts = products.Count;
            ViewBag.LowStockCount = products.Count(x => x.StockQuantity <= 5);
            ViewBag.RecentOrders = orders;

            return View(products);
        }
    }
}
