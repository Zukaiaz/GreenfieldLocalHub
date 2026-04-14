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
    public class BasketProductsController : Controller
    {
        //Holds the database context in _context variable
        private readonly ApplicationDbContext _context;

        //Sets up the database connnection when the controller is created
        public BasketProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BasketProducts
        //shows a list of all basketproducts
        public async Task<IActionResult> Index()
        {
            //Gets all basketProducts and their linked basket and product info
            var applicationDbContext = _context.BasketProducts.Include(b => b.Basket).Include(b => b.Products);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BasketProducts/Details/5
        //Shows the details of one specific basketproduct
        public async Task<IActionResult> Details(int? id)
        {

            // If no ID given, show 404 page
            if (id == null)
            {
                return NotFound();
            }

            // Find the basket product with that ID, also grabbing its basket and product info
            var basketProducts = await _context.BasketProducts
                .Include(b => b.Basket)
                .Include(b => b.Products)
                .FirstOrDefaultAsync(m => m.BasketProductsId == id);
            // If nothing was found, show 404 page
            if (basketProducts == null)
            {
                return NotFound();
            }

            return View(basketProducts);
        }

        // GET: BasketProducts/Create
        // Opens the form to add a new basket product
        public IActionResult Create()
        {
            // Fills the basket dropdown with options
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId");
            // Fills the product dropdown with options
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId");
            return View();
        }

        // POST: BasketProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Saves the new basket product when the form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken]// Security check to block fake form submissions
        public async Task<IActionResult> Create(int ProductsId)
        {
           var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductsId == ProductsId);

            if (product == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (userId == null)
            {
                return Unauthorized();
            }

            var basket = await _context.Basket.FirstOrDefaultAsync(x => x.UserId == userId && x.Status == true);

            if (basket == null)
            {
                basket = new Basket
                {
                    Status = true,
                    UserId = userId,
                    BasketCreatedAt = DateTime.UtcNow,

                };

                _context.Basket.Add(basket);
                await _context.SaveChangesAsync();
            }

            var basketProduct = await _context.BasketProducts
                .FirstOrDefaultAsync(bp => bp.BasketId == basket.BasketId && bp.ProductsId == ProductsId);

            if (basketProduct != null)
            {
                basketProduct.ProductQuantity++;
            }
            else
            {
                basketProduct = new BasketProducts
                {
                    BasketId = basket.BasketId,
                    ProductsId = ProductsId,
                    ProductQuantity = 1
                };

                _context.BasketProducts.Add(basketProduct);

            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Baskets");

        }

        // GET: BasketProducts/Edit/5
        // Opens the edit form for a specific basket product
        public async Task<IActionResult> Edit(int? id)
        {
            // If no ID given, show 404 page
            if (id == null)
            {
                return NotFound();
            }

            // Find the basket product by ID
            var basketProducts = await _context.BasketProducts.FindAsync(id);
            // If nothing was found, show 404 page
            if (basketProducts == null)
            {
                return NotFound();
            }
            // Reload the dropdowns with the current values already selected
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId);
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", basketProducts.ProductsId);
            return View(basketProducts);
        }

        // POST: BasketProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // Saves the changes when the edit form is submitted
        [HttpPost]
        [ValidateAntiForgeryToken] // Security check to block fake form submissions
        public async Task<IActionResult> Edit(int id, [Bind("BasketProductsId,BasketId,ProductsId,ProductQuantity")] BasketProducts basketProducts)
        {
            // Makes sure the ID in the URL matches the record being edited
            if (id != basketProducts.BasketProductsId)
            {
                return NotFound();
            }
            // If the form is filled in correctly, save the changes
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(basketProducts); //Update the record
                    await _context.SaveChangesAsync(); //Save
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If the record was deleted before we could save, show 404
                    if (!BasketProductsExists(basketProducts.BasketProductsId))
                    {
                        return NotFound();
                    }
                    else 
                    {
                        throw; // Something else went wrong, throw the error
                    }
                }
                return RedirectToAction(nameof(Index));  // go back to the list
            }

            // If the form had errors, reload the dropdowns and show the form again
            ViewData["BasketId"] = new SelectList(_context.Basket, "BasketId", "BasketId", basketProducts.BasketId);
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", basketProducts.ProductsId);
            return View(basketProducts);
        }

        // GET: BasketProducts/Delete/5
        // Shows the delete confirmation page for a specific basket product
        public async Task<IActionResult> Delete(int? id)
        {
            // If no ID given, show 404 page
            if (id == null)
            {
                return NotFound();
            }

            // Find the basket product by ID, also grabbing its basket and product info for display
            var basketProducts = await _context.BasketProducts
                .Include(b => b.Basket)
                .Include(b => b.Products)
                .FirstOrDefaultAsync(m => m.BasketProductsId == id);

            // If nothing was found, show 404 page
            if (basketProducts == null)
            {
                return NotFound();
            }

            return View(basketProducts);
        }

        // POST: BasketProducts/Delete/5
        // Actually deletes the basket product after the user confirms
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Security check to block fake form submissions
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //Find the record to delete
            var basketProducts = await _context.BasketProducts.FindAsync(id);

            //If it exists, then delete
            if (basketProducts != null)
            {
                _context.BasketProducts.Remove(basketProducts);
            }

            await _context.SaveChangesAsync(); //Save the deletion
            return RedirectToAction(nameof(Index)); //Go back to the list
        }

        // Checks if a basket product exists in the database by its ID
        private bool BasketProductsExists(int id)
        {
            return _context.BasketProducts.Any(e => e.BasketProductsId == id);
        }
    }
}
