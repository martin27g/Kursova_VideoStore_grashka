using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.OrderDetails
{
    public class DeleteModel : PageModel
    {
        private readonly VideotekaContext _context;

        public DeleteModel(VideotekaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = default!;

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderDetail = await _context.OrderDetails
                .Include(od => od.Film)
                .Include(od => od.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            if (OrderDetail == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = $"Delete {id} failed. Try again.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails.FindAsync(id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            try
            {
                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException)
            {
                return RedirectToPage("./Delete", new { id, saveChangesError = true });
            }
        }
    }
}
