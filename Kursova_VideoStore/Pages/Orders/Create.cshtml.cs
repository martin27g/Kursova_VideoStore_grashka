using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Orders
{
    public class CreateModel : PageModel
    {
        private readonly VideotekaContext _context;

        public CreateModel(VideotekaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email");
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email");
                ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID");
                return Page();
            }

            var emptyOrder = new Order();

            if (await TryUpdateModelAsync<Order>(
                emptyOrder,
                "order",
                o => o.CustomerID,
                o => o.EmployeeID,
                o => o.OrderDate))
            {
                _context.Orders.Add(emptyOrder);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Email");
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID");
            return Page();
        }
    }
}
