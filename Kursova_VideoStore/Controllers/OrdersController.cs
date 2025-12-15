using Kursova_VideoStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Videoteka.Models;

namespace Kursova_VideoStore.Controllers
{
    [Authorize(Roles = Roles.AdminEndUser)]
    public class OrdersController : Controller
    {
        private readonly VideotekaContext _context;

        public OrdersController(VideotekaContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CustomerSortParm"] = sortOrder == "Customer" ? "customer_desc" : "Customer";
            ViewData["EmployeeSortParm"] = sortOrder == "Employee" ? "employee_desc" : "Employee";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            // Include related data (Customer and Employee)
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s => s.Customer.LastName.Contains(searchString)
                                       || s.Customer.FirstName.Contains(searchString)
                                       || s.Employee.LastName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    orders = orders.OrderByDescending(s => s.OrderDate);
                    break;
                case "Customer":
                    orders = orders.OrderBy(s => s.Customer.LastName);
                    break;
                case "customer_desc":
                    orders = orders.OrderByDescending(s => s.Customer.LastName);
                    break;
                case "Employee":
                    orders = orders.OrderBy(s => s.Employee.LastName);
                    break;
                case "employee_desc":
                    orders = orders.OrderByDescending(s => s.Employee.LastName);
                    break;
                default:
                    orders = orders.OrderBy(s => s.OrderDate);
                    break;
            }

            int pageSize = 5;
            return View(await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email");
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,CustomerID,EmployeeID,OrderDate")] Order order)
        {
            // Note: In real scenarios, you might auto-set OrderDate here
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email", order.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName", order.EmployeeID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email", order.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName", order.EmployeeID);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,CustomerID,EmployeeID,OrderDate")] Order order)
        {
            if (id != order.OrderID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email", order.CustomerID);
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "FirstName", order.EmployeeID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null) _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}