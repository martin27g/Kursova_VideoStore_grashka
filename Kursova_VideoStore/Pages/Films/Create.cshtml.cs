using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Films
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
        public Film Film { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var emptyFilm = new Film();

            // Overposting protection: only allow specific properties
            if (await TryUpdateModelAsync<Film>(
                emptyFilm,
                "film", // Prefix for form values
                f => f.Title,
                f => f.Genre,
                f => f.ReleaseYear,
                f => f.Price,
                f => f.Stock))
            {
                _context.Films.Add(emptyFilm);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
