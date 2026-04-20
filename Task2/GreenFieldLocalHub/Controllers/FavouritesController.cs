using System; // Imports basic system functionality like Exception types
using System.Collections.Generic; // Imports support for Lists and Collections
using System.Linq; // Imports data querying tools like .Any()
using System.Threading.Tasks; // Imports support for asynchronous programming (Tasks)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for SelectLists and dropdowns
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models like Favourites and Products

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace
    public class FavouritesController : Controller // Defines the class for managing user Favourites
    { // Start of class
        private readonly ApplicationDbContext _context; // Declares a private variable for the DB connection

        public FavouritesController(ApplicationDbContext context) // Constructor to inject the DB connection
        { // Start of constructor
            _context = context; // Stores the database connection in the local variable
        } // End of constructor

        // GET: Favourites
        [HttpGet] // GET: Identifies this as a request to retrieve and show data
        public async Task<IActionResult> Index() // Method to show all items in the Favourites list
        { // Start of Index
            var applicationDbContext = _context.Favourites.Include(f => f.Products); // Prepares a query that joins Favourites with Products
            return View(await applicationDbContext.ToListAsync()); // Executes the query and sends the list to the view
        } // End of Index

        // GET: Favourites/Details/5
        [HttpGet] // GET: Identifies this as a request to see details for a specific item
        public async Task<IActionResult> Details(int? id) // Method to show one specific favourite record
        { // Start of Details
            if (id == null) // Checks if the ID was missing from the URL
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            var favourites = await _context.Favourites // Searches in the Favourites table
                .Include(f => f.Products) // Includes the Product info so we can see what was favourited
                .FirstOrDefaultAsync(m => m.FavouritesId == id); // Finds the first record matching that ID

            if (favourites == null) // Checks if the database failed to find the record
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            return View(favourites); // Sends the specific record to the details view
        } // End of Details

        // GET: Favourites/Create
        [HttpGet] // GET: Loads the empty "Add to Favourites" page
        public IActionResult Create() // Method to load the creation form
        { // Start of Create
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId"); // Loads product IDs into a dropdown menu
            return View(); // Returns the blank view
        } // End of Create

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // POST: Identifies this as a submission of new data
        [ValidateAntiForgeryToken] // Security check to prevent hackers from submitting fake forms
        public async Task<IActionResult> Create([Bind("FavouritesId,UserId,ProductsId")] Favourites favourites) // Logic to save a favourite
        { // Start of Create POST
            if (ModelState.IsValid) // Checks if the data matches the rules in the Favourites model
            { // Start if
                _context.Add(favourites); // Marks the new favourite for addition to the DB
                await _context.SaveChangesAsync(); // Saves the record to the database
                return RedirectToAction(nameof(Index)); // Redirects user to the list page
            } // End if
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", favourites.ProductsId); // Re-populates dropdown if form was invalid
            return View(favourites); // Shows the form again with error messages
        } // End of Create POST

        // GET: Favourites/Edit/5
        [HttpGet] // GET: Loads the form with existing data to be changed
        public async Task<IActionResult> Edit(int? id) // Method to load the edit page
        { // Start of Edit
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var favourites = await _context.Favourites.FindAsync(id); // Directly looks for the record by ID
            if (favourites == null) // If the record doesn't exist
            { // Start if
                return NotFound(); // Returns 404
            } // End if
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", favourites.ProductsId); // Pre-selects the product in the dropdown
            return View(favourites); // Returns the edit form with the data
        } // End of Edit

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // POST: Submission of changes to an existing record
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> Edit(int id, [Bind("FavouritesId,UserId,ProductsId")] Favourites favourites) // Logic to save edits
        { // Start of Edit POST
            if (id != favourites.FavouritesId) // Checks if the ID in the URL matches the ID in the form
            { // Start if
                return NotFound(); // Returns 404 if there is a mismatch
            } // End if

            if (ModelState.IsValid) // Checks if the new data is valid
            { // Start if
                try // Tries to perform the update
                { // Start try
                    _context.Update(favourites); // Marks the record as updated
                    await _context.SaveChangesAsync(); // Commits the change to the DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handles error if record was changed by someone else at the same time
                { // Start catch
                    if (!FavouritesExists(favourites.FavouritesId)) // Checks if the record was actually deleted
                    { // Start if
                        return NotFound(); // Returns 404
                    } // End if
                    else // If a different error occurred
                    { // Start else
                        throw; // Rethrows the error to be handled globally
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index)); // Returns to the list on success
            } // End if
            ViewData["ProductsId"] = new SelectList(_context.Set<Products>(), "ProductsId", "ProductsId", favourites.ProductsId); // Reloads dropdown if data was invalid
            return View(favourites); // Returns the form with error messages
        } // End of Edit POST

        // GET: Favourites/Delete/5
        [HttpGet] // GET: Shows the confirmation page before deleting
        public async Task<IActionResult> Delete(int? id) // Method to load delete page
        { // Start of Delete
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var favourites = await _context.Favourites // Searches for the record
                .Include(f => f.Products) // Includes product info for the confirmation message
                .FirstOrDefaultAsync(m => m.FavouritesId == id); // Finds the first match

            if (favourites == null) // If the record was not found
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            return View(favourites); // Shows the delete confirmation view
        } // End of Delete

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")] // POST: The final button press to remove the item
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Final logic to remove the favourite
        { // Start of DeleteConfirmed
            var favourites = await _context.Favourites.FindAsync(id); // Finds the record one last time
            if (favourites != null) // If it still exists in the DB
            { // Start if
                _context.Favourites.Remove(favourites); // Marks it for removal
            } // End if

            await _context.SaveChangesAsync(); // Saves the removal to the database
            return RedirectToAction(nameof(Index)); // Goes back to the list page
        } // End of DeleteConfirmed

        private bool FavouritesExists(int id) // Helper method to check if a record exists
        { // Start helper
            return _context.Favourites.Any(e => e.FavouritesId == id); // Returns true if ID is found in the DB
        } // End helper
    } // End of class
} // End of namespace