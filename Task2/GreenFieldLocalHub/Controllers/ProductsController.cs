using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenFieldLocalHub.Data;
using GreenFieldLocalHub.Models;
using System.Security.Claims;

namespace GreenFieldLocalHub.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Farmer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized();
                }

                var farmer = await _context.Farmers.FirstOrDefaultAsync(s => s.UserId == userId);

                if (farmer == null)
                {
                    return NotFound();
                }

                var farmerProducts = await _context.Products
                    .Where(p => p.FarmersId == farmer.FarmersId)
                    .Include(p => p.Farmers)
                    .ToListAsync();

                return View(farmerProducts);
            }
            else
            {
                var allProducts = await _context.Products
                    .Include(p => p.Farmers)
                    .ToListAsync();

                return View(allProducts);
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Farmers)
                .FirstOrDefaultAsync(m => m.ProductsId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["FarmersId"] = new SelectList(_context.Farmers, "FarmersId", "FarmersId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductsId,ProductName,ProductDescription,StockQuantity,ProductPrice,IsAvailable")] Products products, IFormFile? ImageFile)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
            if (farmer == null) return NotFound();

            products.FarmersId = farmer.FarmersId;
            ModelState.Remove("FarmersId");
            ModelState.Remove("Farmers");

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewData["ImageError"] = "Only .jpg, .png, and .webp files are allowed.";
                    return View(products);
                }

                var fileName = Guid.NewGuid() + extension;
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using var stream = new FileStream(savePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream);

                products.ImagePath = "/images/products/" + fileName;
            }
            else
            {
                products.ImagePath = "/images/default.png";
            }

            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(products);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductsId,ProductName,ProductDescription,StockQuantity,ProductPrice,IsAvailable,ImagePath")] Products products, IFormFile? ImageFile)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
            if (farmer == null) return NotFound();

            products.FarmersId = farmer.FarmersId;
            ModelState.Remove("FarmersId");
            ModelState.Remove("Farmers");

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(ImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewData["ImageError"] = "Only .jpg, .png, and .webp files are allowed.";
                    return View(products);
                }

                var fileName = Guid.NewGuid() + extension;
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using var stream = new FileStream(savePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream);

                products.ImagePath = "/images/products/" + fileName;
            }
            else
            {
                products.ImagePath = "/images/default.png";
            }

            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Farmers)
                .FirstOrDefaultAsync(m => m.ProductsId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {


            var products = await _context.Products.FindAsync(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var farmers = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
            if (farmers == null)
            {
                return NotFound();
            }

            products.FarmersId = farmers.FarmersId;
            ModelState.Remove("FarmersId");

            if (products != null)
            {
                _context.Products.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> SidebarPartial()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Content("<p>Please log in to view your basket.</p>", "text/html");

            var basket = await _context.Basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status);

            if (basket == null)
                return Content("<p>Your basket is empty.</p>", "text/html");

            var basketProducts = await _context.BasketProducts
                .Where(x => x.BasketId == basket.BasketId)
                .Include(x => x.Products)
                .ToListAsync();

            if (!basketProducts.Any())
                return Content("<p>Your basket is empty.</p>", "text/html");

            var loyaltyAccount = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(x => x.UserId == userId);

            decimal subtotal = basketProducts.Sum(x => x.Products.ProductPrice * x.ProductQuantity);

            decimal discountPercent = loyaltyAccount?.Tier switch
            {
                "Bronze" => 0.05m,
                "Silver" => 0.10m,
                "Gold" => 0.15m,
                _ => 0m
            };

            decimal discountAmount = subtotal * discountPercent;

            ViewBag.Subtotal = subtotal;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.Total = subtotal - discountAmount;
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None";

            return PartialView("SidebarPartial", basketProducts);
        }

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductsId == id);
        }
    }
}
