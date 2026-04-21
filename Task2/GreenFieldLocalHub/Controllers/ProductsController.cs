using System; // Imports basic system types
using System.Collections.Generic; // Imports support for lists
using System.Linq; // Imports data querying tools
using System.Threading.Tasks; // Imports support for async/await
using Microsoft.AspNetCore.Mvc; // Imports MVC framework components
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for dropdown lists
using Microsoft.EntityFrameworkCore; // Imports the database engine
using GreenFieldLocalHub.Data; // Imports your database context
using GreenFieldLocalHub.Models; // Imports your data models
using System.Security.Claims; // Imports tools to identify the logged-in user

namespace GreenFieldLocalHub.Controllers // The container for this controller
{ // Start of namespace
    public class ProductsController : Controller // Defines the class for managing the product catalog
    { // Start of class
        private readonly ApplicationDbContext _context; // Variable for database access

        public ProductsController(ApplicationDbContext context) // Constructor: Sets up the controller
        { // Start of constructor
            _context = context; // Links the database to the local variable
        } // End of constructor

        // GET: Products
        public async Task<IActionResult> Index() // Method to list products
        { // Start of Index
            if (User.IsInRole("Farmer")) // Check if the logged-in user is a Farmer
            { // Start if Farmer
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the Farmer's ID

                if (userId == null) // If no user ID is found
                { // Start if
                    return Unauthorized(); // Block access
                } // End if

                var farmer = await _context.Farmers.FirstOrDefaultAsync(s => s.UserId == userId); // Look up the farmer's profile

                if (farmer == null) // If no profile exists for that ID
                { // Start if
                    return NotFound(); // Return 404
                } // End if

                var farmerProducts = await _context.Products // Get products from the database
                    .Where(p => p.FarmersId == farmer.FarmersId) // Only get products belonging to THIS farmer
                    .Include(p => p.Farmers) // Join farmer details
                    .ToListAsync(); // Run the query

                return View(farmerProducts); // Show only the farmer's products
            } // End if Farmer
            else // If the user is a Customer or Admin
            { // Start else
                var allProducts = await _context.Products // Get all products from the database
                    .Include(p => p.Farmers) // Join farmer info (to show who grew it)
                    .ToListAsync(); // Run the query

                return View(allProducts); // Show the full catalog
            } // End else
        } // End of Index

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id) // Method to show one product's info
        { // Start of Details
            if (id == null) // Check if ID is missing
            { // Start if
                return NotFound(); // Return 404
            } // End if

            var products = await _context.Products // Search the Products table
                .Include(p => p.Farmers) // Join the Farmer's info
                .FirstOrDefaultAsync(m => m.ProductsId == id); // Find the matching product
            if (products == null) // If product doesn't exist
            { // Start if
                return NotFound(); // Return 404
            } // End if

            return View(products); // Show the product details view
        } // End of Details

        // GET: Products/Create
        public IActionResult Create() // Method to load the "Add New Product" form
        { // Start of Create GET
            ViewData["FarmersId"] = new SelectList(_context.Farmers, "FarmersId", "FarmersId"); // Setup dropdown for farmers
            return View(); // Return the blank form
        } // End of Create GET

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Handle form submission
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> Create([Bind("ProductsId,ProductName,ProductDescription,StockQuantity,ProductPrice,IsAvailable")] Products products, IFormFile? ImageFile) // Logic to save product and image
        { // Start of Create POST
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID
            if (userId == null) return Unauthorized(); // Check login

            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId); // Link product to farmer
            if (farmer == null) return NotFound(); // Check profile

            products.FarmersId = farmer.FarmersId; // Assign ownership to the logged-in farmer
            ModelState.Remove("FarmersId"); // Bypass manual ID entry validation
            ModelState.Remove("Farmers"); // Bypass navigation property validation

            if (ImageFile != null && ImageFile.Length > 0) // Logic for handling image uploads
            { // Start if Image
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" }; // Define safe file types
                var extension = Path.GetExtension(ImageFile.FileName).ToLower(); // Get the file extension

                if (!allowedExtensions.Contains(extension)) // Check if file type is allowed
                { // Start if invalid
                    ViewData["ImageError"] = "Only .jpg, .png, and .webp files are allowed."; // Set error message
                    return View(products); // Reload form with error
                } // End if invalid

                var fileName = Guid.NewGuid() + extension; // Give the image a unique random name
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName); // Set folder path

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!); // Ensure the folder exists

                using var stream = new FileStream(savePath, FileMode.Create); // Create the file on the server
                await ImageFile.CopyToAsync(stream); // Save the image data

                products.ImagePath = "/images/products/" + fileName; // Save the web path in the database
            } // End if Image
            else // If no image was uploaded
            { // Start else
                products.ImagePath = "/images/default.png"; // Use a placeholder image
            } // End else

            if (ModelState.IsValid) // If all data is valid
            { // Start if valid
                _context.Add(products); // Add to DB tracker
                await _context.SaveChangesAsync(); // Save to database
                return RedirectToAction(nameof(Index)); // Go back to list
            } // End if valid

            return View(products); // Return form with errors if validation failed
        } // End of Create POST

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id) // Load the edit form
        { // Start of Edit GET
            if (id == null) // Check ID
            { // Start if
                return NotFound(); // 404
            } // End if

            var products = await _context.Products.FindAsync(id); // Find the product
            if (products == null) // Check existence
            { // Start if
                return NotFound(); // 404
            } // End if

            return View(products); // Return the filled-out form
        } // End of Edit GET

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Handle edit submission
        [ValidateAntiForgeryToken] // Security
        public async Task<IActionResult> Edit(int id, [Bind("ProductsId,ProductName,ProductDescription,StockQuantity,ProductPrice,IsAvailable,ImagePath")] Products products, IFormFile? ImageFile) // Logic to update
        { // Start of Edit POST

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID
            if (userId == null) return Unauthorized(); // Check login

            var farmer = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId); // Get farmer profile
            if (farmer == null) return NotFound(); // Check existence

            products.FarmersId = farmer.FarmersId; // Re-assign farmer ID
            ModelState.Remove("FarmersId"); // Clean validation
            ModelState.Remove("Farmers"); // Clean validation

            if (ImageFile != null && ImageFile.Length > 0) // Handle new image if provided
            { // Start if Image
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" }; // Allowed types
                var extension = Path.GetExtension(ImageFile.FileName).ToLower(); // Get extension

                if (!allowedExtensions.Contains(extension)) // Check extension
                { // Start if invalid
                    ViewData["ImageError"] = "Only .jpg, .png, and .webp files are allowed."; // Error msg
                    return View(products); // Reload
                } // End if invalid

                var fileName = Guid.NewGuid() + extension; // Generate unique name
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName); // Set path

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!); // Ensure folder exists

                using var stream = new FileStream(savePath, FileMode.Create); // Create file
                await ImageFile.CopyToAsync(stream); // Save file

                products.ImagePath = "/images/products/" + fileName; // Update DB path
            } // End if Image
            // Note: If no new image, it keeps the existing ImagePath from the Bind attribute

            if (ModelState.IsValid) // Check validity
            { // Start if valid
                _context.Update(products); // Update record
                await _context.SaveChangesAsync(); // Save to DB
                return RedirectToAction(nameof(Index)); // Return to list
            } // End if valid

            return View(products); // Reload form with errors
        } // End of Edit POST

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id) // Show delete confirmation
        { // Start of Delete GET
            if (id == null) // Check ID
            { // Start if
                return NotFound(); // 404
            } // End if

            var products = await _context.Products // Find product
                .Include(p => p.Farmers) // Join farmer info
                .FirstOrDefaultAsync(m => m.ProductsId == id); // Search
            if (products == null) // Check existence
            { // Start if
                return NotFound(); // 404
            } // End if

            return View(products); // Return confirmation view
        } // End of Delete GET

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")] // Handler for delete button click
        [ValidateAntiForgeryToken] // Security
        public async Task<IActionResult> DeleteConfirmed(int id) // Logic to remove product
        { // Start of DeleteConfirmed
            var products = await _context.Products.FindAsync(id); // Find the product

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID
            if (userId == null) // Security check
            { // Start if
                return Unauthorized(); // Block if not logged in
            } // End if

            var farmers = await _context.Farmers.FirstOrDefaultAsync(f => f.UserId == userId); // Get farmer
            if (farmers == null) // Check profile
            { // Start if
                return NotFound(); // 404
            } // End if

            products.FarmersId = farmers.FarmersId; // Ensure context for removal
            ModelState.Remove("FarmersId"); // Clean validation

            if (products != null) // If product found
            { // Start if
                _context.Products.Remove(products); // Mark for removal
            } // End if

            await _context.SaveChangesAsync(); // Commit to DB
            return RedirectToAction(nameof(Index)); // Back to list
        } // End of DeleteConfirmed

        public async Task<IActionResult> SidebarPartial() // Logic for the shopping basket sidebar
        { // Start of SidebarPartial
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID

            if (userId == null) // If logged out
                return Content("<p>Please log in to view your basket.</p>", "text/html"); // Show simple text

            var basket = await _context.Basket // Find the open basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status); // Check ID and status

            if (basket == null) // If no active basket
                return Content("<p>Your basket is empty.</p>", "text/html"); // Show simple text

            var basketProducts = await _context.BasketProducts // Get items in the basket
                .Where(x => x.BasketId == basket.BasketId) // Filter by ID
                .Include(x => x.Products) // Join product details
                .ToListAsync(); // Run query

            if (!basketProducts.Any()) // If basket exists but has 0 items
                return Content("<p>Your basket is empty.</p>", "text/html"); // Show simple text

            var loyaltyAccount = await _context.LoyaltyAccount // Look for loyalty tier
                .FirstOrDefaultAsync(x => x.UserId == userId); // Filter by user

            decimal subtotal = basketProducts.Sum(x => x.Products.ProductPrice * x.ProductQuantity); // Calculate subtotal

            decimal discountPercent = loyaltyAccount?.Tier switch // Calculate discount based on tier
            { // Start switch
                "Bronze" => 0.05m, // 5%
                "Silver" => 0.10m, // 10%
                "Gold" => 0.15m, // 15%
                _ => 0m // 0%
            }; // End switch

            decimal discountAmount = subtotal * discountPercent; // Calculate discount value

            ViewBag.Subtotal = subtotal; // Pass to view
            ViewBag.DiscountAmount = discountAmount; // Pass to view
            ViewBag.Total = subtotal - discountAmount; // Pass final total
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Pass tier name

            return PartialView("SidebarPartial", basketProducts); // Return the small sidebar snippet view
        } // End of SidebarPartial

        private bool ProductsExists(int id) // Helper tool to check if product exists
        { // Start helper
            return _context.Products.Any(e => e.ProductsId == id); // True if found
        } // End helper

        // This handles the search request
        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index"); // If search is empty, just show the shop
            }

            // Look for a product where the name contains the search text
            // .FirstOrDefaultAsync() gets the very first match it finds
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductName.Contains(query));

            if (product != null)
            {
                // If a product is found, take them directly to the Details page of that product
                return RedirectToAction("Details", new { id = product.ProductsId });
            }

            // If nothing is found, send them to the Shop Index (maybe show a "Not Found" message)
            TempData["Message"] = "No product found matching: " + query;
            return RedirectToAction("Index");
        }

    } // End of class
} // End of namespace