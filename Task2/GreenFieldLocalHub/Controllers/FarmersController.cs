using System; // Imports basic system functionality like types and dates
using System.Collections.Generic; // Imports support for lists and collections
using System.Linq; // Imports data querying tools (like .Any())
using System.Threading.Tasks; // Imports support for asynchronous tasks (async/await)
using Microsoft.AspNetCore.Mvc; // Imports the Model-View-Controller framework classes
using Microsoft.AspNetCore.Mvc.Rendering; // Imports tools for rendering HTML elements
using Microsoft.EntityFrameworkCore; // Imports the database engine for C#
using GreenFieldLocalHub.Data; // Imports your project's database context
using GreenFieldLocalHub.Models; // Imports your data models (Farmers, etc.)

namespace GreenFieldLocalHub.Controllers // Defines the container for this controller
{ // Start of namespace
    public class FarmersController : Controller // Defines the class that handles Farmer profiles
    { // Start of class
        private readonly ApplicationDbContext _context; // Declares a private variable for the database connection

        public FarmersController(ApplicationDbContext context) // Constructor: Runs when the controller is made
        { // Start of constructor
            _context = context; // Stores the database connection in the local variable
        } // End of constructor

        // GET: Farmers
        public async Task<IActionResult> Index() // Method to show a list of all farmers
        { // Start of Index
            return View(await _context.Farmers.ToListAsync()); // Fetches all farmers from the DB and sends them to the Index page
        } // End of Index

        // GET: Farmers/Details/5
        public async Task<IActionResult> Details(int? id) // Method to show specific info for one farmer
        { // Start of Details
            if (id == null) // Checks if the ID was missing from the URL
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            var farmers = await _context.Farmers // Looks into the Farmers table
                .FirstOrDefaultAsync(m => m.FarmersId == id); // Finds the first farmer matching the ID
            if (farmers == null) // Checks if the database failed to find a record
            { // Start if
                return NotFound(); // Returns 404 error
            } // End if

            return View(farmers); // Sends the farmer data to the Details page
        } // End of Details

        // GET: Farmers/Create
        public IActionResult Create() // Method to load the "Add Farmer" form
        { // Start of Create
            return View(); // Returns the blank form view
        } // End of Create

        // POST: Farmers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // Defines this as a POST request (submitting data)
        [ValidateAntiForgeryToken] // Security check to prevent CSRF attacks
        public async Task<IActionResult> Create([Bind("FarmersId,UserId,FarmerName,FarmerEmail,FarmingMethod,FarmerInfo")] Farmers farmers) // Logic to save a new farmer
        { // Start of Create POST
            if (ModelState.IsValid) // Checks if the submitted data follows the model rules
            { // Start if
                _context.Add(farmers); // Prepares the new farmer for the database
                await _context.SaveChangesAsync(); // Saves the new record
                return RedirectToAction(nameof(Index)); // Sends user back to the list
            } // End if
            return View(farmers); // If data was bad, returns the form with the current entries
        } // End of Create POST

        // GET: Farmers/Edit/5
        public async Task<IActionResult> Edit(int? id) // Method to load the edit form for a farmer
        { // Start of Edit
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var farmers = await _context.Farmers.FindAsync(id); // Directly looks for the farmer by ID
            if (farmers == null) // If farmer record doesn't exist
            { // Start if
                return NotFound(); // Returns 404
            } // End if
            return View(farmers); // Returns the edit form with the farmer's data
        } // End of Edit

        // POST: Farmers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost] // POST request to update data
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> Edit(int id, [Bind("FarmersId,UserId,FarmerName,FarmerEmail,FarmingMethod,FarmerInfo")] Farmers farmers) // Logic to save changes
        { // Start of Edit POST
            if (id != farmers.FarmersId) // Security check: Does URL ID match the Form ID?
            { // Start if
                return NotFound(); // Returns 404 if mismatch
            } // End if

            if (ModelState.IsValid) // Checks if edited data is valid
            { // Start if
                try // Tries to update
                { // Start try
                    _context.Update(farmers); // Marks the record as modified
                    await _context.SaveChangesAsync(); // Saves the update to the DB
                } // End try
                catch (DbUpdateConcurrencyException) // Handles errors if record was changed elsewhere
                { // Start catch
                    if (!FarmersExists(farmers.FarmersId)) // Checks if the farmer was deleted during the edit
                    { // Start if
                        return NotFound(); // Returns 404
                    } // End if
                    else // If a different error occurred
                    { // Start else
                        throw; // Rethrows the error
                    } // End else
                } // End catch
                return RedirectToAction(nameof(Index)); // Goes back to the list on success
            } // End if
            return View(farmers); // If data was invalid, stays on form with error messages
        } // End of Edit POST

        // GET: Farmers/Delete/5
        public async Task<IActionResult> Delete(int? id) // Method to load delete confirmation page
        { // Start of Delete
            if (id == null) // Checks if ID is missing
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            var farmers = await _context.Farmers // Searches for the farmer
                .FirstOrDefaultAsync(m => m.FarmersId == id); // Finds matching record
            if (farmers == null) // If not found
            { // Start if
                return NotFound(); // Returns 404
            } // End if

            return View(farmers); // Shows the confirmation page
        } // End of Delete

        // POST: Farmers/Delete/5
        [HttpPost, ActionName("Delete")] // POST request mapped to the "Delete" confirmation button
        [ValidateAntiForgeryToken] // Security check
        public async Task<IActionResult> DeleteConfirmed(int id) // Final logic to remove farmer
        { // Start of DeleteConfirmed
            var farmers = await _context.Farmers.FindAsync(id); // Finds the record one last time
            if (farmers != null) // If it exists
            { // Start if
                _context.Farmers.Remove(farmers); // Marks it for deletion
            } // End if

            await _context.SaveChangesAsync(); // Saves the removal in the database
            return RedirectToAction(nameof(Index)); // Goes back to the list
        } // End of DeleteConfirmed

        private bool FarmersExists(int id) // Private tool to check if a record is in the DB
        { // Start helper
            return _context.Farmers.Any(e => e.FarmersId == id); // Returns true if ID is found
        } // End helper
    } // End of class
} // End of namespace