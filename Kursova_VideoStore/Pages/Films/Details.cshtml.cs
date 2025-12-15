using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Films
{
    public class DetailsModel : PageModel
    {
        private readonly VideotekaContext _context;

        public DetailsModel(VideotekaContext context)
        {
            _context = context;
        }

        public Film Film { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films
                .Include(f => f.OrderDetails)
                    .ThenInclude(od => od.Order)
                        .ThenInclude(o => o.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FilmID == id);

            if (film == null)
            {
                return NotFound();
            }

            Film = film;
            return Page();
        }
    }
}
