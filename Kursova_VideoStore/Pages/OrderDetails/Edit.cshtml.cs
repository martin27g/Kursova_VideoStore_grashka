using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.OrderDetails
{
    public class EditModel : PageModel
    {
        private readonly VideotekaContext _context;

        public EditModel(VideotekaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Film)
                .Include(od => od.Order)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            OrderDetail = orderDetail;
            PopulateDropDowns();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetailToUpdate = await _context.OrderDetails.FindAsync(id);
            if (orderDetailToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<OrderDetail>(
                orderDetailToUpdate,
                "orderDetail",
                od => od.OrderID,
                od => od.FilmID,
                od => od.Quantity))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetailToUpdate.OrderDetailID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Repopulate dropdowns if validation fails
            PopulateDropDowns();
            return Page();
        }

        private void PopulateDropDowns()
        {
            ViewData["FilmID"] = new SelectList(_context.Films, "FilmID", "Title");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailID == id);
        }
    }
}
