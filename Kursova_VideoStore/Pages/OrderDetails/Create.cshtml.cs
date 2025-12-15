using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.OrderDetails
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
            ViewData["FilmID"] = new SelectList(_context.Films, "FilmID", "Title");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return Page();
        }

        [BindProperty]
        public OrderDetail OrderDetail { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["FilmID"] = new SelectList(_context.Films, "FilmID", "Title");
                ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
                return Page();
            }

            var emptyOrderDetail = new OrderDetail();

            if (await TryUpdateModelAsync<OrderDetail>(
                emptyOrderDetail,
                "orderDetail",
                od => od.OrderID,
                od => od.FilmID,
                od => od.Quantity))
            {
                _context.OrderDetails.Add(emptyOrderDetail);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            ViewData["FilmID"] = new SelectList(_context.Films, "FilmID", "Title");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return Page();
        }
    }
}
