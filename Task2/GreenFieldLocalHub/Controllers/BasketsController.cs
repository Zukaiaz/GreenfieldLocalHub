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
    public class BasketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BasketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Baskets
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var basket = await _context.Basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status);

            if (basket == null)
            {
                {
                    basket = new Basket
                    {
                        Status = true,
                        UserId = userId,
                        BasketCreatedAt = DateTime.UtcNow
                    };

                    _context.Basket.Add(basket);
                    await _context.SaveChangesAsync();
                }
            }

            var basketProducts = await _context.BasketProducts
                .Where(x => x.BasketId == basket.BasketId)
                .Include(x => x.Basket)
                .Include(x => x.Products)
                .ToListAsync();

            decimal subtotal = 0m;

            foreach (var basketProduct in basketProducts)
            {
                var productTotal = basketProduct.Products.ProductPrice * basketProduct.ProductQuantity;
                subtotal += productTotal;
            }

            // Get the users loyalty account
            var loyaltyAccount = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(x => x.UserId == userId);

            // Work out discount based on their tier
            decimal discountPercent = 0m;

            if (loyaltyAccount != null)
            {
                discountPercent = loyaltyAccount.Tier switch
                {
                    "Bronze" => 0.05m,  // 5% off
                    "Silver" => 0.10m,  // 10% off
                    "Gold" => 0.15m,  // 15% off
                    _ => 0m      // No discount for standard users
                };
            }

            decimal discountAmount = subtotal * discountPercent;
            decimal total = subtotal - discountAmount;

            // Pass values to the view
            ViewBag.Subtotal = subtotal;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.Total = total;
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None";

            return View(basketProducts);
        }

        // GET: Baskets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .FirstOrDefaultAsync(m => m.BasketId == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // GET: Baskets/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Baskets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BasketId,Status,BasketCreatedAt,UserId")] Basket basket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(basket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            

            return View(basket);
        }

        // GET: Baskets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket.FindAsync(id);
            if (basket == null)
            {
                return NotFound();
            }
            return View(basket);
        }

        // POST: Baskets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BasketId,Status,BasketCreatedAt,UserId")] Basket basket)
        {
            if (id != basket.BasketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BasketExists(basket.BasketId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(basket);
        }

        // GET: Baskets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var basket = await _context.Basket
                .FirstOrDefaultAsync(m => m.BasketId == id);
            if (basket == null)
            {
                return NotFound();
            }

            return View(basket);
        }

        // POST: Baskets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var basket = await _context.Basket.FindAsync(id);
            if (basket != null)
            {
                _context.Basket.Remove(basket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BasketExists(int id)
        {
            return _context.Basket.Any(e => e.BasketId == id);
        }
        public async Task<IActionResult> GetTotals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var basket = await _context.Basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status);

            if (basket == null)
                return Json(new { subtotal = "0.00", discountAmount = "0.00", total = "0.00" });

            var basketProducts = await _context.BasketProducts
                .Where(x => x.BasketId == basket.BasketId)
                .Include(x => x.Products)
                .ToListAsync();

            decimal subtotal = basketProducts.Sum(x => x.Products.ProductPrice * x.ProductQuantity);

            var loyaltyAccount = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(x => x.UserId == userId);

            decimal discountPercent = loyaltyAccount?.Tier switch
            {
                "Bronze" => 0.05m,
                "Silver" => 0.10m,
                "Gold" => 0.15m,
                _ => 0m
            };

            decimal discountAmount = subtotal * discountPercent;
            decimal total = subtotal - discountAmount;

            return Json(new
            {
                subtotal = subtotal.ToString("0.00"),
                discountAmount = discountAmount.ToString("0.00"),
                total = total.ToString("0.00")
            });
        }

    }
}
