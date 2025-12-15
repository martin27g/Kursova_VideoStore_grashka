using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Customers
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
        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var emptyCustomer = new Customer();

            // Overposting protection: only allow specific properties
            if (await TryUpdateModelAsync<Customer>(
                emptyCustomer,
                "customer", // Prefix for form values
                c => c.FirstName,
                c => c.LastName,
                c => c.Email,
                c => c.Phone,
                c => c.Address))
            {
                _context.Customers.Add(emptyCustomer);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            // If TryUpdateModelAsync fails, redisplay form
            return Page();
        }
    }
}
