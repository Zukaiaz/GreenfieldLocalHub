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
    public class LoyaltyTransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoyaltyTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LoyaltyTransactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LoyaltyTransactions.Include(l => l.LoyaltyAccount).Include(l => l.Orders);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LoyaltyTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyTransactions = await _context.LoyaltyTransactions
                .Include(l => l.LoyaltyAccount)
                .Include(l => l.Orders)
                .FirstOrDefaultAsync(m => m.LoyaltyTransactionsId == id);
            if (loyaltyTransactions == null)
            {
                return NotFound();
            }

            return View(loyaltyTransactions);
        }

        // GET: LoyaltyTransactions/Create
        public IActionResult Create()
        {
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId");
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId");
            return View();
        }

        // POST: LoyaltyTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoyaltyTransactionsId,LoyaltyAccountId,OrdersId,PointsChange,Reason,CreatedAt")] LoyaltyTransactions loyaltyTransactions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loyaltyTransactions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId);
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId);
            return View(loyaltyTransactions);
        }

        // GET: LoyaltyTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyTransactions = await _context.LoyaltyTransactions.FindAsync(id);
            if (loyaltyTransactions == null)
            {
                return NotFound();
            }
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId);
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId);
            return View(loyaltyTransactions);
        }

        // POST: LoyaltyTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoyaltyTransactionsId,LoyaltyAccountId,OrdersId,PointsChange,Reason,CreatedAt")] LoyaltyTransactions loyaltyTransactions)
        {
            if (id != loyaltyTransactions.LoyaltyTransactionsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loyaltyTransactions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoyaltyTransactionsExists(loyaltyTransactions.LoyaltyTransactionsId))
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
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId);
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId);
            return View(loyaltyTransactions);
        }

        // GET: LoyaltyTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyTransactions = await _context.LoyaltyTransactions
                .Include(l => l.LoyaltyAccount)
                .Include(l => l.Orders)
                .FirstOrDefaultAsync(m => m.LoyaltyTransactionsId == id);
            if (loyaltyTransactions == null)
            {
                return NotFound();
            }

            return View(loyaltyTransactions);
        }

        // POST: LoyaltyTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loyaltyTransactions = await _context.LoyaltyTransactions.FindAsync(id);
            if (loyaltyTransactions != null)
            {
                _context.LoyaltyTransactions.Remove(loyaltyTransactions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoyaltyTransactionsExists(int id)
        {
            return _context.LoyaltyTransactions.Any(e => e.LoyaltyTransactionsId == id);
        }
    }
}
