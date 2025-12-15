using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Employees
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
            return Page();
        }

        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var emptyEmployee = new Employee();

            // Overposting protection: only allow specific properties
            if (await TryUpdateModelAsync<Employee>(
                emptyEmployee,
                "employee", // Prefix for form values
                e => e.FirstName,
                e => e.LastName,
                e => e.Position,
                e => e.HireDate))
            {
                _context.Employees.Add(emptyEmployee);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // If TryUpdateModelAsync fails, redisplay form
            return Page();
        }
    }
}
