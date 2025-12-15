using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Films
{
    public class EditModel : PageModel
    {
        private readonly VideotekaContext _context;

        public EditModel(VideotekaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Film Film { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            Film = film;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var filmToUpdate = await _context.Films.FindAsync(id);
            if (filmToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Film>(
                filmToUpdate,
                "film",
                f => f.Title,
                f => f.Genre,
                f => f.ReleaseYear,
                f => f.Price,
                f => f.Stock))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(filmToUpdate.FilmID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Page();
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.FilmID == id);
        }
    }
}
