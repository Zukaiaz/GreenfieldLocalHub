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
using Microsoft.AspNetCore.Authorization;

namespace GreenFieldLocalHub.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (User.IsInRole("Admin"))
            {
                var allOrders = await _context.Orders
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Products)
                    .ToListAsync();

                return View(allOrders);
            }
            else if (User.IsInRole("Farmer"))
            {
                var supplierProducts = await _context.Products
                    .Where(p => p.Farmers.UserId == userId)
                    .Select(p => p.ProductsId)
                    .ToListAsync(); // Find all supplier products first

                var supplierOrders = await _context.OrderProducts
                    .Where(op => supplierProducts.Contains(op.ProductsId))
                    .Include(op => op.Orders)
                    .Include(op => op.Products)
                    .ToListAsync(); // Now use the supplier products to find supplier orders

                return View(supplierOrders.Select(op => op.Orders).Distinct().ToList());
            }
            else
            {
                var userOrders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Products)
                    .ToListAsync();

                return View(userOrders);
            }
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.OrderProducts
                .Where(op => op.OrdersId == id)
                .Include(op => op.Orders)
                .Include(op => op.Products)
                .ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        [Authorize]
        public async Task<IActionResult> Create(int basketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var basket = await _context.Basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == true);

            if (basket == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var basketProducts = await _context.BasketProducts
                .Where(x => x.BasketId == basket.BasketId)
                .Include(x => x.Products)
                .ToListAsync();

            decimal subtotal = 0.00m;
            foreach (var basketProduct in basketProducts)
            {
                subtotal += basketProduct.Products.ProductPrice * basketProduct.ProductQuantity;
            }

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

            ViewBag.BasketId = basket.BasketId;
            ViewBag.Subtotal = subtotal;
            ViewBag.DiscountAmount = discountAmount;
            ViewBag.Total = total;
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None";
            ViewBag.BasketProducts = basketProducts;

            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryType,CollectionDate")] Orders orders, int basketId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                ViewBag.BasketId = basketId;
                return View(orders);
            }

            orders.UserId = userId;
            ModelState.Remove("UserId");

            orders.OrderDate = DateOnly.FromDateTime(DateTime.Today);
            ModelState.Remove("OrderDate");

            orders.OrderTrackingStatus = "Pending";
            ModelState.Remove("OrderTrackingStatus");

            var basket = await _context.Basket
                .FirstOrDefaultAsync(x => x.BasketId == basketId && x.UserId == userId && x.Status);

            if (basket == null)
            {
                return NotFound();

            }

            var basketProducts = await _context.BasketProducts
                .Where(x => x.BasketId == basketId)
                .Include(x => x.Products)
                .ToListAsync();

            if (!basketProducts.Any())
            {
                ModelState.AddModelError("", "Your basket is empty");
                ViewBag.BasketId = basketId;
                return View(orders);
            }

            decimal subtotal = 0.00m;
            foreach (var basketProduct in basketProducts)
            {
                var productTotal = basketProduct.Products.ProductPrice * basketProduct.ProductQuantity;
                subtotal = productTotal + subtotal;
            }

            var orderCount = await _context.Orders.CountAsync(x => x.UserId == userId);

            decimal discount = 0m;

            if (orderCount >= 5)
            {
                discount = subtotal * 0.10m;
            }

            orders.TotalAmount = subtotal - discount;

            ModelState.Remove("subtotal");

            if (!orders.Collection && !orders.Delivery)
            {
                ModelState.AddModelError("Delivery", "Must choose Collection or Delivery");

            }

            if (orders.Delivery)
            {
                ModelState.Remove("DeliveryType");

                if (orders.CollectionDate == null)
                {
                    ModelState.AddModelError("CollectionDate", "Collection date is Required");

                }

                else
                {
                    var earliestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

                    if (orders.CollectionDate.Value < earliestDate)
                    {
                        ModelState.AddModelError("CollectionDate", "Collection must be at least 2 days from now");
                    }
                }
            }

            if (orders.Delivery)
            {
                ModelState.Remove("CollectionDate");

                if (string.IsNullOrWhiteSpace(orders.DeliveryType))
                {
                    ModelState.AddModelError("DeliveryType", "Delivery type is required");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.BasketId = basketId;
                return View(orders);
            }

            _context.Orders.Add(orders);
            await _context.SaveChangesAsync();

            foreach (var basketProduct in  basketProducts)
            {
                if (basketProduct.Products.StockQuantity < basketProduct.ProductQuantity)
                {
                    ModelState.AddModelError("", $"Not enough stock for {basketProduct.Products.ProductName}");
                    ViewBag.BasketId = basketId;
                    return View(orders);
                }

                var orderProduct = new OrderProducts
                {
                    OrdersId = orders.OrdersId,
                    ProductsId = basketProduct.ProductsId,
                    ProductsQuantity = basketProduct.ProductQuantity,
                };

                _context.OrderProducts.Add(orderProduct);

                basketProduct.Products.StockQuantity -= basketProduct.ProductQuantity;
            }

            basket.Status = false;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");

        }

        // GET: Orders/Edit/5
        [Authorize(Roles = "Farmer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: Orders/Edit/5
        [Authorize(Roles = "Farmer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,UserId,TotalAmount,Delivery,Collection,DeliveryType,OrderTrackingStatus,CollectionDate,OrderDate")] Orders orders)
        {
            if (id != orders.OrdersId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdersExists(orders.OrdersId))
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
            return View(orders);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orders = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrdersId == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orders = await _context.Orders.FindAsync(id);
            if (orders != null)
            {
                _context.Orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdersExists(int id)
        {
            return _context.Orders.Any(e => e.OrdersId == id);
        }
    }
}