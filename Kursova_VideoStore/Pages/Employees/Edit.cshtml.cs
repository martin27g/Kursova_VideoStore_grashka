using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly VideotekaContext _context;

        public EditModel(VideotekaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            Employee = employee;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeToUpdate = await _context.Employees.FindAsync(id);
            if (employeeToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Employee>(
                employeeToUpdate,
                "employee", // Prefix for form values
                e => e.FirstName,
                e => e.LastName,
                e => e.Position,
                e => e.HireDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employeeToUpdate.EmployeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // If TryUpdateModelAsync fails, redisplay form
            return Page();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }
    }
}
