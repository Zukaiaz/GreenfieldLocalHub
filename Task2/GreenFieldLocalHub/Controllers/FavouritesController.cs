using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenFieldLocalHub.Data;
using GreenFieldLocalHub.Models;

namespace GreenFieldLocalHub.Controllers
{
    public class FavouritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavouritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Favourites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Favourites.Include(f => f.Products);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Favourites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourites = await _context.Favourites
                .Include(f => f.Products)
                .FirstOrDefaultAsync(m => m.FavouritesId == id);
            if (favourites == null)
            {
                return NotFound();
            }

            return View(favourites);
        }

        // GET: Favourites/Create
        public IActionResult Create()
        {
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId");
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FavouritesId,UserId,ProductsId")] Favourites favourites)
        {
            if (ModelState.IsValid)
            {
                _context.Add(favourites);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", favourites.ProductsId);
            return View(favourites);
        }

        // GET: Favourites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourites = await _context.Favourites.FindAsync(id);
            if (favourites == null)
            {
                return NotFound();
            }
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", favourites.ProductsId);
            return View(favourites);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FavouritesId,UserId,ProductsId")] Favourites favourites)
        {
            if (id != favourites.FavouritesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(favourites);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FavouritesExists(favourites.FavouritesId))
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
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", favourites.ProductsId);
            return View(favourites);
        }

        // GET: Favourites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var favourites = await _context.Favourites
                .Include(f => f.Products)
                .FirstOrDefaultAsync(m => m.FavouritesId == id);
            if (favourites == null)
            {
                return NotFound();
            }

            return View(favourites);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var favourites = await _context.Favourites.FindAsync(id);
            if (favourites != null)
            {
                _context.Favourites.Remove(favourites);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavouritesExists(int id)
        {
            return _context.Favourites.Any(e => e.FavouritesId == id);
        }
    }
}
