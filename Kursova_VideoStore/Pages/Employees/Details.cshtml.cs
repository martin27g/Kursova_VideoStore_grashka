using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Employees
{
    public class DetailsModel : PageModel
    {
        private readonly VideotekaContext _context;

        public DetailsModel(VideotekaContext context)
        {
            _context = context;
        }

        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Orders)
                    .ThenInclude(o => o.Customer)
                .Include(e => e.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Film)
                .AsNoTracking() // performance optimization
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (employee == null)
            {
                return NotFound();
            }

            Employee = employee;
            return Page();
        }
    }
}
