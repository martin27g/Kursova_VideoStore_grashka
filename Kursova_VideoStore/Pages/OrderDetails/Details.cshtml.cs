using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.OrderDetails
{
    public class DetailsModel : PageModel
    {
        private readonly VideotekaContext _context;

        public DetailsModel(VideotekaContext context)
        {
            _context = context;
        }

        public OrderDetail OrderDetail { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(od => od.Film)                        // load Film info
                .Include(od => od.Order)
                    .ThenInclude(o => o.Customer)             // load Customer info
                .Include(od => od.Order)
                    .ThenInclude(o => o.Employee)             // load Employee info
                .AsNoTracking()                               // performance optimization
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            OrderDetail = orderDetail;
            return Page();
        }
    }
}
