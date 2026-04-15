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
        public async Task<IActionResult> Create([Bind("ProductsId,FarmersId,ProductName,ProductDescription,StockQuantity,ProductPrice,IsAvailable,ImagePath")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmersId"] = new SelectList(_context.Farmers, "FarmersId", "FarmersId", products.FarmersId);
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
        public async Task<IActionResult> Edit(int id, [Bind("ProductsId,ProductName,ProductDescription,StockQuantity,ProductPrice,IsAvailable,ImagePath")] Products products)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var farmers = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId);
            if(farmers == null)
            {
                return NotFound();
            }

            products.FarmersId = farmers.FarmersId;
            ModelState.Remove("FarmersId");

            if (id != products.ProductsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductsId))
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
            ViewData["FarmersId"] = new SelectList(_context.Farmers, "FarmersId", "FarmersId", products.FarmersId);
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

        private bool ProductsExists(int id)
        {
            return _context.Products.Any(e => e.ProductsId == id);
        }
    }
}
