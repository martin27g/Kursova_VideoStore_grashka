using Kursova_VideoStore.Data;
using Kursova_VideoStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Videoteka.Models;

namespace Kursova_VideoStore.Controllers
{
    [Authorize(Roles = Roles.AdminEndUser)]
    public class FilmsController : Controller
    {
        private readonly VideotekaContext _context;

        public FilmsController(VideotekaContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["GenreSortParm"] = sortOrder == "Genre" ? "genre_desc" : "Genre";
            ViewData["YearSortParm"] = sortOrder == "Year" ? "year_desc" : "Year";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var films = _context.Films.Where(f => f.IsActive).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                films = films.Where(s => s.Title.Contains(searchString)
                                       || s.Genre.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    films = films.OrderByDescending(s => s.Title);
                    break;
                case "Genre":
                    films = films.OrderBy(s => s.Genre);
                    break;
                case "genre_desc":
                    films = films.OrderByDescending(s => s.Genre);
                    break;
                case "Year":
                    films = films.OrderBy(s => s.ReleaseYear);
                    break;
                case "year_desc":
                    films = films.OrderByDescending(s => s.ReleaseYear);
                    break;
                case "Price":
                    films = films.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    films = films.OrderByDescending(s => s.Price);
                    break;
                default:
                    films = films.OrderBy(s => s.Title);
                    break;
            }

            int pageSize = 9;
            return View(await PaginatedList<Film>.CreateAsync(films.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var film = await _context.Films.FirstOrDefaultAsync(m => m.FilmID == id);

            if (film == null) return NotFound();

            return View(film);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmID,Title,Genre,Description,ReleaseYear,Price,Stock,IsActive")] Film film)
        {
            if (ModelState.IsValid)
            {
                film.IsActive = true;
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var film = await _context.Films.FindAsync(id);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FilmID,Title,Genre,Description,ReleaseYear,Price,Stock,IsActive")] Film film)
        {
            if (id != film.FilmID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.FilmID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var film = await _context.Films.FirstOrDefaultAsync(m => m.FilmID == id);
            if (film == null) return NotFound();
            return View(film);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film != null)
            {
                film.IsActive = false;
                _context.Update(film);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.FilmID == id);
        }
    }
}