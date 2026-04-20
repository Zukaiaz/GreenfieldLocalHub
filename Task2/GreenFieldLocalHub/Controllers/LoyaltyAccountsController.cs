using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GreenFieldLocalHub.Data;
using GreenFieldLocalHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GreenFieldLocalHub.Controllers
{
    public class LoyaltyAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LoyaltyAccountsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        // GET: Shows the loyalty sign up form
        [Authorize]
        public async Task<IActionResult> SignUp()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If they already have an account, send them straight to MyAccount
            var existing = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (existing != null)
            {
                return RedirectToAction(nameof(MyAccount));
            }

            return View();
        }

        // POST: Handles the loyalty sign up form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(string email)
        {
            // Find the user by the email they entered
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Error = "No account found with that email address.";
                return View();
            }

            // Check if they already have a loyalty account
            var existing = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existing != null)
            {
                ViewBag.Error = "You already have a loyalty account!";
                return View();
            }

            // Create their loyalty account
            var loyaltyAccount = new LoyaltyAccount
            {
                UserId = user.Id,
                Points = 0,
                Tier = "None",
                CreatedAt = DateTime.UtcNow
            };

            _context.LoyaltyAccount.Add(loyaltyAccount);
            await _context.SaveChangesAsync();

            ViewBag.Success = "You have successfully signed up to our loyalty scheme!";
            return View();
        }

        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var loyaltyAccount = await _context.LoyaltyAccount
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (loyaltyAccount == null)
            {
                return RedirectToAction(nameof(SignUp));
            }

            // Get their transaction history
            var transactions = await _context.LoyaltyTransactions
                .Where(x => x.LoyaltyAccountId == loyaltyAccount.LoyaltyAccountId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.Transactions = transactions;
            return View(loyaltyAccount);
        }

        private bool LoyaltyAccountExists(int id)
        {
            return _context.LoyaltyAccount.Any(e => e.LoyaltyAccountId == id);
        }
    }
}