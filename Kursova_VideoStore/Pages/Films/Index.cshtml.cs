using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Films
{
    public class IndexModel : PageModel
    {
        private readonly VideotekaContext _context;

        public IndexModel(VideotekaContext context)
        {
            _context = context;
        }

        // Sorting
        public string CurrentSort { get; set; }
        public string TitleSort { get; set; }
        public string GenreSort { get; set; }
        public string YearSort { get; set; }
        public string PriceSort { get; set; }
        public string StockSort { get; set; }

        // Filtering
        public string CurrentFilter { get; set; }

        // Paging
        public PaginatedList<Film> Films { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            TitleSort = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            GenreSort = sortOrder == "Genre" ? "genre_desc" : "Genre";
            YearSort = sortOrder == "Year" ? "year_desc" : "Year";
            PriceSort = sortOrder == "Price" ? "price_desc" : "Price";
            StockSort = sortOrder == "Stock" ? "stock_desc" : "Stock";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Film> filmIQ = from f in _context.Films
                                      select f;

            // Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                filmIQ = filmIQ.Where(f => f.Title.Contains(searchString)
                                        || f.Genre.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "title_desc":
                    filmIQ = filmIQ.OrderByDescending(f => f.Title);
                    break;
                case "Genre":
                    filmIQ = filmIQ.OrderBy(f => f.Genre);
                    break;
                case "genre_desc":
                    filmIQ = filmIQ.OrderByDescending(f => f.Genre);
                    break;
                case "Year":
                    filmIQ = filmIQ.OrderBy(f => f.ReleaseYear);
                    break;
                case "year_desc":
                    filmIQ = filmIQ.OrderByDescending(f => f.ReleaseYear);
                    break;
                case "Price":
                    filmIQ = filmIQ.OrderBy(f => f.Price);
                    break;
                case "price_desc":
                    filmIQ = filmIQ.OrderByDescending(f => f.Price);
                    break;
                case "Stock":
                    filmIQ = filmIQ.OrderBy(f => f.Stock);
                    break;
                case "stock_desc":
                    filmIQ = filmIQ.OrderByDescending(f => f.Stock);
                    break;
                default:
                    filmIQ = filmIQ.OrderBy(f => f.Title);
                    break;
            }

            // Paging
            int pageSize = 5;
            Films = await PaginatedList<Film>.CreateAsync(
                filmIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
