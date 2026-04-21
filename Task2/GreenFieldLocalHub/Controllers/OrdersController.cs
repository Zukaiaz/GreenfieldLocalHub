using System; // Imports basic system functionality
using System.Collections.Generic; // Imports support for Lists and Collections
using System.Linq; // Imports LINQ for filtering and selecting data
using System.Threading.Tasks; // Imports support for asynchronous programming
using Microsoft.AspNetCore.Mvc; // Imports MVC controller and action result classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML elements like dropdowns
using Microsoft.EntityFrameworkCore; // Imports Entity Framework for database operations
using GreenFieldLocalHub.Data; // Imports your ApplicationDbContext
using GreenFieldLocalHub.Models; // Imports your data models
using System.Security.Claims; // Imports tools to retrieve the logged-in user's ID
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Imports security attributes like [Authorize]

namespace GreenFieldLocalHub.Controllers // Defines the namespace for this controller
{ // Start of namespace
    public class OrdersController : Controller // Defines the OrdersController class inheriting from Controller
    { // Start of class
        private readonly ApplicationDbContext _context; // Private variable for database access
        private readonly UserManager<IdentityUser> _userManager; // Private variable for database

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager) // Constructor to inject the database context
        { // Start of constructor
            _context = context; // Assigns the injected context to the private variable
            _userManager = userManager;
        } // End of constructor

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index() // Method to list orders based on user role
        { // Start of Index
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets the unique ID of the current user
            if (userId == null) // Checks if the user is not logged in
            { // Start if
                return Unauthorized(); // Returns a 401 Unauthorized status
            } // End if
            if (User.IsInRole("Admin")) // Logic for Administrator users
            { // Start if Admin
                var allOrders = await _context.Orders // Queries all orders in the system
                    .Include(o => o.OrderProducts) // Includes the link to products
                    .ThenInclude(op => op.Products) // Includes the actual product details
                    .ToListAsync(); // Executes the query and returns the list

                var adminUsers = await _userManager.Users.ToListAsync(); // Gets all registered users from the identity table
                ViewBag.UserEmails = adminUsers.ToDictionary(u => u.Id, u => u.Email); // Builds a userId to email lookup dictionary for the view

                return View(allOrders); // Sends all orders to the view
            } // End if Admin
            else if (User.IsInRole("Farmer")) // Logic for Farmer/Supplier users
            { // Start if Farmer
                var supplierProducts = await _context.Products // Queries the Products table
                    .Where(p => p.Farmers.UserId == userId) // Filters for products belonging to this farmer
                    .Select(p => p.ProductsId) // Only grabs the IDs of those products
                    .ToListAsync(); // Executes query to find supplier products first
                var supplierOrders = await _context.OrderProducts // Queries the linking table
                    .Where(op => supplierProducts.Contains(op.ProductsId)) // Finds rows where the product belongs to this farmer
                    .Include(op => op.Orders) // Joins the parent Order info
                        .ThenInclude(o => o.OrderProducts) // For each order, also loads all its linked OrderProducts rows
                            .ThenInclude(op => op.Products) // For each of those OrderProducts, loads the full product details
                    .Include(op => op.Products) // Joins the Product info
                    .ToListAsync(); // Executes query to find relevant orders

                var farmerUsers = await _userManager.Users.ToListAsync(); // Gets all registered users from the identity table
                ViewBag.UserEmails = farmerUsers.ToDictionary(u => u.Id, u => u.Email); // Builds a userId to email lookup dictionary so the farmer can see whose order it is

                return View(supplierOrders.Select(op => op.Orders).Distinct().ToList()); // Returns a unique list of orders containing the farmer's products
            } // End if Farmer
            else // Logic for standard Customers
            { // Start else
                var userOrders = await _context.Orders // Queries the Orders table
                    .Where(o => o.UserId == userId) // Filters only for orders matching the current user's ID
                    .Include(o => o.OrderProducts) // Includes the products in those orders
                    .ThenInclude(op => op.Products) // Includes full product details
                    .ToListAsync(); // Executes query
                return View(userOrders); // Sends the user's personal order history to the view — no email lookup needed as customers only see their own orders
            } // End else
        } // End of Index
        [Authorize]
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id) // Method to display the full details of a specific order
        { // Start of Details
            if (id == null) // Checks if no ID was provided in the URL
                return NotFound(); // Returns a 404 error if ID is missing

            var orders = await _context.OrderProducts // Queries the OrderProducts linking table
                .Where(op => op.OrdersId == id) // Filters for rows that belong to this specific order ID
                .Include(op => op.Orders) // Joins the parent Order record so we can access order details
                .Include(op => op.Products) // Joins the Product record so we can access product name and price
                .ToListAsync(); // Executes the query and returns the results as a list

            if (orders == null || !orders.Any()) // Checks if no order products were found
                return NotFound(); // Returns a 404 error if the order doesn't exist

            var userId = orders.First().Orders.UserId; // Gets the ID of the user who placed this order from the first row

            var loyaltyAccount = await _context.LoyaltyAccount // Queries the LoyaltyAccount table
                .FirstOrDefaultAsync(x => x.UserId == userId); // Finds the loyalty record belonging to this order's user

            decimal discountPercent = loyaltyAccount?.Tier switch // Uses a switch to pick the discount based on their tier
            {
                "Bronze" => 0.05m, // Bronze members get 5% off
                "Silver" => 0.10m, // Silver members get 10% off
                "Gold" => 0.15m, // Gold members get 15% off
                _ => 0m     // Anyone else gets no discount
            };

            decimal total = orders.First().Orders.TotalAmount; // Gets the final total that was saved when the order was placed
            decimal subtotal = discountPercent > 0 ? total / (1 - discountPercent) : total; // Reverses the discount maths to work out what the original subtotal was before the discount was applied. If no discount, subtotal equals total.
            decimal discountAmount = subtotal - total; // Calculates the actual cash amount that was discounted by subtracting the total from the subtotal
            ViewBag.DiscountAmount = discountAmount; // Passes the discount amount to the view so it can be displayed
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Passes the tier name to the view, or "None" if they have no loyalty account
            return View(orders); // Sends the list of order products to the Details view
        } // End of Details
        [Authorize]
        // GET: Orders/Create
        [Authorize] // Restricts access to logged-in users
        public async Task<IActionResult> Create(int basketId) // Method to load the checkout/order creation page
        { // Start of Create GET
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets current user ID

            var basket = await _context.Basket // Looks for the user's active basket
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Status == true); // Checks ID and that the basket is "Open"

            if (basket == null) // If no active basket is found
            { // Start if
                return RedirectToAction("Index", "Products"); // Redirects back to shopping
            } // End if

            var basketProducts = await _context.BasketProducts // Gets items currently in the basket
                .Where(x => x.BasketId == basket.BasketId) // Filters by basket ID
                .Include(x => x.Products) // Joins product info to get prices
                .ToListAsync(); // Executes query

            decimal subtotal = 0.00m; // Initializes subtotal variable
            foreach (var basketProduct in basketProducts) // Loops through each item in basket
            { // Start foreach
                subtotal += basketProduct.Products.ProductPrice * basketProduct.ProductQuantity; // Calculates price x quantity and adds to total
            } // End foreach

            var loyaltyAccount = await _context.LoyaltyAccount // Looks for the user's loyalty record
                .FirstOrDefaultAsync(x => x.UserId == userId); // Matches by User ID

            decimal discountPercent = loyaltyAccount?.Tier switch // Determines discount percentage based on tier name
            { // Start switch
                "Bronze" => 0.05m, // 5% discount
                "Silver" => 0.10m, // 10% discount
                "Gold" => 0.15m, // 15% discount
                _ => 0m // 0% for "None" or unknown
            }; // End switch

            decimal discountAmount = subtotal * discountPercent; // Calculates the cash value of the discount
            decimal total = subtotal - discountAmount; // Subtracts discount from subtotal for the final price

            ViewBag.BasketId = basket.BasketId; // Passes Basket ID to the view
            ViewBag.Subtotal = subtotal; // Passes Subtotal to the view
            ViewBag.DiscountAmount = discountAmount; // Passes Discount to the view
            ViewBag.Total = total; // Passes Final Total to the view
            ViewBag.Tier = loyaltyAccount?.Tier ?? "None"; // Passes Tier name to the view
            ViewBag.BasketProducts = basketProducts; // Passes the list of items to the view

            return View(); // Returns the checkout page view
        } // End of Create GET
        [Authorize]
        // POST: Orders/Create
        [HttpPost] // Marks this as a form submission handler
        [Authorize] // Requires login
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> Create([Bind("OrdersId,Delivery,Collection,DeliveryType,CollectionDate")] Orders orders, int basketId) // Saves the order
        { // Start of Create POST
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Gets User ID

            if (userId == null) // Check for session loss
            { // Start if
                ViewBag.BasketId = basketId; // Keeps basket ID for reload
                return View(orders); // Returns view with data
            } // End if

            orders.UserId = userId; // Assigns the order to the current user
            ModelState.Remove("UserId"); // Removes validation requirement for UserId since we set it manually

            orders.OrderDate = DateOnly.FromDateTime(DateTime.Today); // Sets order date to today
            ModelState.Remove("OrderDate"); // Removes validation requirement

            orders.OrderTrackingStatus = "Pending"; // Sets initial status
            ModelState.Remove("OrderTrackingStatus"); // Removes validation requirement

            var basket = await _context.Basket // Finds the basket being checked out
                .FirstOrDefaultAsync(x => x.BasketId == basketId && x.UserId == userId && x.Status); // Ensures it's theirs and still open

            if (basket == null) // Error handling for missing basket
            { // Start if
                return NotFound(); // Returns 404

            } // End if

            var basketProducts = await _context.BasketProducts // Gets items to convert to order items
                .Where(x => x.BasketId == basketId) // Filter by ID
                .Include(x => x.Products) // Include product data
                .ToListAsync(); // Execute query

            if (!basketProducts.Any()) // Logic for empty basket checkout attempt
            { // Start if
                @ModelState.AddModelError("", "Your basket is empty"); // Adds error message
                ViewBag.BasketId = basketId; // Keeps ID for reload
                return View(orders); // Reloads the view
            } // End if

            decimal subtotal = 0.00m; // Subtotal calculation
            foreach (var basketProduct in basketProducts) // Loop items
            { // Start foreach
                var productTotal = basketProduct.Products.ProductPrice * basketProduct.ProductQuantity; // Calculate line total
                subtotal = productTotal + subtotal; // Accumulate subtotal
            } // End foreach

            var loyaltyAccount = await _context.LoyaltyAccount // Get loyalty data
            .FirstOrDefaultAsync(x => x.UserId == userId); // Filter by user

            decimal discountPercent = loyaltyAccount?.Tier switch // Determine discount again for final calculation
            { // Start switch
                "Bronze" => 0.05m, // 5%
                "Silver" => 0.10m, // 10%
                "Gold" => 0.15m, // 15%
                _ => 0m // 0%
            }; // End switch

            decimal discount = subtotal * discountPercent; // Calculate discount
            orders.TotalAmount = subtotal - discount; // Set final amount on the Order object

            ModelState.Remove("subtotal"); // Cleans up validation tracking

            if (!orders.Collection && !orders.Delivery) // Validation: User must pick a method
            { // Start if
                ModelState.AddModelError("Delivery", "Must choose Collection or Delivery"); // Adds error

            } // End if

            if (orders.Delivery) // Logic for delivery choice
            { // Start if Delivery
                ModelState.Remove("DeliveryType"); // Cleans up validation

                if (orders.CollectionDate == null) // Validation: needs a date
                { // Start if
                    ModelState.AddModelError("CollectionDate", "Collection date is Required"); // Adds error

                } // End if

                else // Date range validation
                { // Start else
                    var earliestDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)); // Sets limit to 2 days from now

                    if (orders.CollectionDate.Value < earliestDate) // Check if date is too soon
                    { // Start if
                        ModelState.AddModelError("CollectionDate", "Collection must be at least 2 days from now"); // Adds error
                    } // End if
                } // End else
            } // End if Delivery

            if (orders.Delivery) // Additional delivery validation
            { // Start if
                ModelState.Remove("CollectionDate"); // Cleans up validation

                if (string.IsNullOrWhiteSpace(orders.DeliveryType)) // Check if type (e.g. Standard/Express) is empty
                { // Start if
                    ModelState.AddModelError("DeliveryType", "Delivery type is required"); // Adds error
                } // End if
            } // End if

            if (!ModelState.IsValid) // Final check for all errors
            { // Start if
                ViewBag.BasketId = basketId; // Retain ID
                return View(orders); // Return view with errors
            } // End if

            _context.Orders.Add(orders); // Adds the Order header to DB tracker
            await _context.SaveChangesAsync(); // Saves Order to get the OrdersId

            foreach (var basketProduct in basketProducts) // Loop to move items from basket to order
            { // Start foreach
                if (basketProduct.Products.StockQuantity < basketProduct.ProductQuantity) // Inventory check
                { // Start if
                    ModelState.AddModelError("", $"Not enough stock for {basketProduct.Products.ProductName}"); // Adds error
                    ViewBag.BasketId = basketId; // Retain ID
                    return View(orders); // Return view
                } // End if

                var orderProduct = new OrderProducts // Create new OrderItem record
                { // Start assignment
                    OrdersId = orders.OrdersId, // Links to the newly created Order ID
                    ProductsId = basketProduct.ProductsId, // Copies product ID
                    ProductsQuantity = basketProduct.ProductQuantity, // Copies quantity
                }; // End assignment

                _context.OrderProducts.Add(orderProduct); // Adds link to DB tracker

                basketProduct.Products.StockQuantity -= basketProduct.ProductQuantity; // Reduces actual store stock
            } // End foreach

            basket.Status = false; // "Closes" the basket so it can't be reused
            await _context.SaveChangesAsync(); // Commits items and stock changes

            // Award loyalty points — 10 points per £1 spent (based on subtotal before discount)
            if (loyaltyAccount != null) // Only award if they have an account
            { // Start if
                int pointsEarned = (int)(subtotal * 10); // Calculates points
                loyaltyAccount.Points += pointsEarned; // Adds to account balance

                // Update tier based on new total points
                loyaltyAccount.Tier = loyaltyAccount.Points switch // Checks for tier upgrades
                { // Start switch
                    >= 1000 => "Gold", // 1000+ points
                    >= 600 => "Silver", // 600-999 points
                    >= 300 => "Bronze", // 300-599 points
                    _ => loyaltyAccount.Tier  // No change if below 300
                }; // End switch

                var transaction = new LoyaltyTransactions // Log the point gain
                { // Start assignment
                    LoyaltyAccountId = loyaltyAccount.LoyaltyAccountId, // Link to account
                    OrdersId = orders.OrdersId, // Link to this order
                    PointsChange = pointsEarned, // Amount gained
                    Reason = $"Order #{orders.OrdersId} — £{subtotal:F2} spent", // Description
                    CreatedAt = DateTime.UtcNow // Timestamp
                }; // End assignment

                _context.LoyaltyTransactions.Add(transaction); // Adds log to DB tracker
                await _context.SaveChangesAsync(); // Final save for loyalty data
            } // End if

            return RedirectToAction("Index", "Home"); // Redirects to homepage on success

        } // End of Create POST
        [Authorize]
        // GET: Orders/Edit/5
        [Authorize(Roles = "Farmer")] // Only Farmers can edit (likely to change tracking status)
        public async Task<IActionResult> Edit(int? id) // Method to load edit form
        { // Start of Edit GET
            if (id == null) // Check ID
            { // Start if
                return NotFound(); // 404
            } // End if

            var orders = await _context.Orders.FindAsync(id); // Find order header
            if (orders == null) // Check existence
            { // Start if
                return NotFound(); // 404
            } // End if
            return View(orders); // Return edit view
        } // End of Edit GET
        [Authorize]
        // POST: Orders/Edit/5
        [Authorize(Roles = "Farmer")] // Restrict to Farmers
        [HttpPost] // Submit handler
        [ValidateAntiForgeryToken] // Security
        public async Task<IActionResult> Edit(int id, [Bind("OrdersId,UserId,TotalAmount,Delivery,Collection,DeliveryType,OrderTrackingStatus,CollectionDate,OrderDate")] Orders orders) // Logic to save edits
        { // Start of Edit POST
            if (id != orders.OrdersId) // ID verification
            { // Start if
                return NotFound(); // 404
            } // End if

            if (ModelState.IsValid) // Check data validity
            { // Start if
                try // Attempt update
                { // Start try
                    _context.Update(orders); // Mark as modified
                    await _context.SaveChangesAsync(); // Save to DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handle multi-user collision
                { // Start catch
                    if (!OrdersExists(orders.OrdersId)) // Check if deleted
                    { // Start if
                        return NotFound(); // 404
                    } // End if
                    else // Rethrow unknown error
                    { // Start else
                        throw; // Crash/Log
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index)); // Back to list on success
            } // End if
            return View(orders); // Return form with errors
        } // End of Edit POST
        [Authorize]
        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id) // Method to load delete confirmation
        { // Start of Delete GET
            if (id == null) // Check ID
            { // Start if
                return NotFound(); // 404
            } // End if

            var orders = await _context.Orders // Find order
                .FirstOrDefaultAsync(m => m.OrdersId == id); // Execute
            if (orders == null) // Check existence
            { // Start if
                return NotFound(); // 404
            } // End if

            return View(orders); // Return confirmation view
        } // End of Delete GET
        [Authorize]
        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")] // Handler for final delete click
        [ValidateAntiForgeryToken] // Security
        public async Task<IActionResult> DeleteConfirmed(int id) // Logic to remove order
        { // Start of DeleteConfirmed
            var orders = await _context.Orders.FindAsync(id); // Find record
            if (orders != null) // Check existence
            { // Start if
                _context.Orders.Remove(orders); // Mark for removal
            } // End if

            await _context.SaveChangesAsync(); // Commit removal
            return RedirectToAction(nameof(Index)); // Back to list
        } // End of DeleteConfirmed

        private bool OrdersExists(int id) // Helper for DB checks
        { // Start helper
            return _context.Orders.Any(e => e.OrdersId == id); // Returns true if ID found
        } // End helper


        [HttpPost] // Tells the server this code only runs when a form is submitted
        [Authorize(Roles = "Farmer")] // Only allows users with the "Farmer" role to access this logic
        [ValidateAntiForgeryToken] // Security check to make sure the request came from your actual site
        public async Task<IActionResult> UpdateStatus(int id, string status) // The function that receives the Order ID and new Status
        { // Start of the update process

            var order = await _context.Orders.FindAsync(id); // Looks in the database to find the specific order using its ID number

            if (order == null) // If the order doesn't exist
                return NotFound(); // Stop and show an error page

            order.OrderTrackingStatus = status; // Overwrites the old status in the database with the new one from the dropdown

            await _context.SaveChangesAsync(); // Saves the change permanently to the database

            return RedirectToAction("Details", new { id = id }); // Sends the user back to the "Details" page for that order to see the change
        } // End of the update process
    } // End of class
} // End of namespace