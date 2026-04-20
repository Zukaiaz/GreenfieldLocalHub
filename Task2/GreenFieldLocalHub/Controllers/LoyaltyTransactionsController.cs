using System; // Imports basic system functionality
using System.Collections.Generic; // Imports support for Lists and Collections
using System.Linq; // Imports data querying tools like .Any()
using System.Threading.Tasks; // Imports support for asynchronous programming (async/await)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for SelectLists (dropdowns)
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models (LoyaltyTransactions, etc.)

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace
    public class LoyaltyTransactionsController : Controller // Defines the class for managing loyalty point history
    { // Start of class
        private readonly ApplicationDbContext _context; // Declares a private variable for the database connection

        public LoyaltyTransactionsController(ApplicationDbContext context) // Constructor to inject the database context
        { // Start of constructor
            _context = context; // Stores the database connection in the local variable
        } // End of constructor

        // GET: LoyaltyTransactions
        [HttpGet] // GET: Identifies this as a request to display the transaction history list
        public async Task<IActionResult> Index() // Method to show all loyalty transactions
        { // Start of Index
            var applicationDbContext = _context.LoyaltyTransactions // Queries the Transactions table
                .Include(l => l.LoyaltyAccount) // Joins with the LoyaltyAccount table to see who the points belong to
                .Include(l => l.Orders); // Joins with the Orders table to see which purchase caused the points change
            return View(await applicationDbContext.ToListAsync()); // Executes the query and sends the list to the Index View
        } // End of Index

        // GET: LoyaltyTransactions/Details/5
        [HttpGet] // GET: Identifies this as a request for specific transaction details
        public async Task<IActionResult> Details(int? id) // Method to show one specific transaction record
        { // Start of Details
            if (id == null) // Checks if the ID was missing from the URL
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            var loyaltyTransactions = await _context.LoyaltyTransactions // Searches the Transactions table
                .Include(l => l.LoyaltyAccount) // Includes the account info
                .Include(l => l.Orders) // Includes the order info
                .FirstOrDefaultAsync(m => m.LoyaltyTransactionsId == id); // Finds the first record matching the ID

            if (loyaltyTransactions == null) // Checks if the record was not found
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            return View(loyaltyTransactions); // Sends the specific transaction data to the Details View
        } // End of Details

        // GET: LoyaltyTransactions/Create
        [HttpGet] // GET: Loads the empty "Manual Transaction" form
        public IActionResult Create() // Method to display the creation form
        { // Start of Create
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId"); // Creates a dropdown for Account IDs
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId"); // Creates a dropdown for Order IDs
            return View(); // Returns the blank Create View
        } // End of Create

        // POST: LoyaltyTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // POST: Identifies this as a submission of a new transaction
        [ValidateAntiForgeryToken] // Security layer to prevent CSRF attacks
        public async Task<IActionResult> Create([Bind("LoyaltyTransactionsId,LoyaltyAccountId,OrdersId,PointsChange,Reason,CreatedAt")] LoyaltyTransactions loyaltyTransactions) // Logic to save a transaction
        { // Start of Create POST
            if (ModelState.IsValid) // Checks if the submitted data is valid according to the model rules
            { // Start if
                _context.Add(loyaltyTransactions); // Marks the new transaction for addition
                await _context.SaveChangesAsync(); // Saves the transaction to the database
                return RedirectToAction(nameof(Index)); // Returns to the history list
            } // End if
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId); // Reloads account dropdown on error
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId); // Reloads order dropdown on error
            return View(loyaltyTransactions); // Shows form again with errors
        } // End of Create POST

        // GET: LoyaltyTransactions/Edit/5
        [HttpGet] // GET: Loads existing transaction data for editing
        public async Task<IActionResult> Edit(int? id) // Method to load the edit form
        { // Start of Edit
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var loyaltyTransactions = await _context.LoyaltyTransactions.FindAsync(id); // Finds the record by its primary key
            if (loyaltyTransactions == null) // If the record doesn't exist
            { // Start if
                return NotFound(); // Returns 404
            } // End if
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId); // Pre-selects account in dropdown
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId); // Pre-selects order in dropdown
            return View(loyaltyTransactions); // Returns the edit form with data
        } // End of Edit

        // POST: LoyaltyTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // POST: Submission of changes
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> Edit(int id, [Bind("LoyaltyTransactionsId,LoyaltyAccountId,OrdersId,PointsChange,Reason,CreatedAt")] LoyaltyTransactions loyaltyTransactions) // Logic to save edits
        { // Start of Edit POST
            if (id != loyaltyTransactions.LoyaltyTransactionsId) // Security check: Does URL ID match Form ID?
            { // Start if
                return NotFound(); // Returns 404 if mismatch
            } // End if

            if (ModelState.IsValid) // Checks if the updated data is valid
            { // Start if
                try // Tries to update the database
                { // Start try
                    _context.Update(loyaltyTransactions); // Marks record as modified
                    await _context.SaveChangesAsync(); // Saves changes to the DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handles errors if record was changed by another process
                { // Start catch
                    if (!LoyaltyTransactionsExists(loyaltyTransactions.LoyaltyTransactionsId)) // Checks if record was deleted
                    { // Start if
                        return NotFound(); // Returns 404
                    } // End if
                    else // If a different DB error occurred
                    { // Start else
                        throw; // Rethrows the error
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index)); // Goes back to list on success
            } // End if
            ViewData["LoyaltyAccountId"] = new SelectList(_context.LoyaltyAccount, "LoyaltyAccountId", "LoyaltyAccountId", loyaltyTransactions.LoyaltyAccountId); // Reloads dropdowns on error
            ViewData["OrdersId"] = new SelectList(_context.Set<Orders>(), "OrdersId", "OrdersId", loyaltyTransactions.OrdersId); // Reloads dropdowns on error
            return View(loyaltyTransactions); // Returns form with error messages
        } // End of Edit POST

        // GET: LoyaltyTransactions/Delete/5
        [HttpGet] // GET: Loads confirmation page for deletion
        public async Task<IActionResult> Delete(int? id) // Method to load delete page
        { // Start of Delete
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var loyaltyTransactions = await _context.LoyaltyTransactions // Searches for record
                .Include(l => l.LoyaltyAccount) // Includes account info for user clarity
                .Include(l => l.Orders) // Includes order info
                .FirstOrDefaultAsync(m => m.LoyaltyTransactionsId == id); // Finds the matching transaction

            if (loyaltyTransactions == null) // If not found
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            return View(loyaltyTransactions); // Shows the confirmation View
        } // End of Delete

        // POST: LoyaltyTransactions/Delete/5
        [HttpPost, ActionName("Delete")] // POST: Triggered by the final Delete button
        [ValidateAntiForgeryToken] // Security layer
        public async Task<IActionResult> DeleteConfirmed(int id) // Final logic to remove transaction
        { // Start of DeleteConfirmed
            var loyaltyTransactions = await _context.LoyaltyTransactions.FindAsync(id); // Finds the record
            if (loyaltyTransactions != null) // If it exists
            { // Start if
                _context.LoyaltyTransactions.Remove(loyaltyTransactions); // Marks it for deletion
            } // End if

            await _context.SaveChangesAsync(); // Saves the removal to the database
            return RedirectToAction(nameof(Index)); // Goes back to the list
        } // End of DeleteConfirmed

        private bool LoyaltyTransactionsExists(int id) // Helper method to check existence in DB
        { // Start helper
            return _context.LoyaltyTransactions.Any(e => e.LoyaltyTransactionsId == id); // Returns true if ID is found
        } // End helper
    } // End of class
} // End of namespace