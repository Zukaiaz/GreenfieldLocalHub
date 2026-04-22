using System.Diagnostics;
using GreenFieldLocalHub.Models;
using Microsoft.AspNetCore.Mvc;
using GreenFieldLocalHub.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenFieldLocalHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var featuredProducts = await _context.Products
                .Include(p => p.Farmers)
                .OrderBy(p => Guid.NewGuid())
                .Take(4)
                .ToListAsync();

            return View(featuredProducts);
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}