using Kursova_VideoStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Videoteka.Models; // Required for PaginatedList and Customer

namespace Kursova_VideoStore.Controllers
{
    [Authorize(Roles = Roles.AdminEndUser)] // Strict Admin Access
    public class CustomersController : Controller
    {
        private readonly VideotekaContext _context;

        public CustomersController(VideotekaContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var customers = _context.Customers.AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                customers = customers.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString)
                                       || s.Email.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(s => s.LastName);
                    break;
                case "Email":
                    customers = customers.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    customers = customers.OrderByDescending(s => s.Email);
                    break;
                default:
                    customers = customers.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Customer>.CreateAsync(customers.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,FirstName,LastName,Email,Phone,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,FirstName,LastName,Email,Phone,Address")] Customer customer)
        {
            if (id != customer.CustomerID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var customer = await _context.Customers.FirstOrDefaultAsync(m => m.CustomerID == id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null) _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerID == id);
        }
    }
}