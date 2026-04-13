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
    public class LoyaltyAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoyaltyAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LoyaltyAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoyaltyAccount.ToListAsync());
        }

        // GET: LoyaltyAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyAccount = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(m => m.LoyaltyAccountId == id);
            if (loyaltyAccount == null)
            {
                return NotFound();
            }

            return View(loyaltyAccount);
        }

        // GET: LoyaltyAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoyaltyAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoyaltyAccountId,UserId,Points,Tier,CreatedAt")] LoyaltyAccount loyaltyAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loyaltyAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loyaltyAccount);
        }

        // GET: LoyaltyAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyAccount = await _context.LoyaltyAccount.FindAsync(id);
            if (loyaltyAccount == null)
            {
                return NotFound();
            }
            return View(loyaltyAccount);
        }

        // POST: LoyaltyAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoyaltyAccountId,UserId,Points,Tier,CreatedAt")] LoyaltyAccount loyaltyAccount)
        {
            if (id != loyaltyAccount.LoyaltyAccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loyaltyAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoyaltyAccountExists(loyaltyAccount.LoyaltyAccountId))
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
            return View(loyaltyAccount);
        }

        // GET: LoyaltyAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loyaltyAccount = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(m => m.LoyaltyAccountId == id);
            if (loyaltyAccount == null)
            {
                return NotFound();
            }

            return View(loyaltyAccount);
        }

        // POST: LoyaltyAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loyaltyAccount = await _context.LoyaltyAccount.FindAsync(id);
            if (loyaltyAccount != null)
            {
                _context.LoyaltyAccount.Remove(loyaltyAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoyaltyAccountExists(int id)
        {
            return _context.LoyaltyAccount.Any(e => e.LoyaltyAccountId == id);
        }
    }
}
