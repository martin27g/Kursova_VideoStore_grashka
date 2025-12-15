using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Customers
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
        public string NameSort { get; set; }
        public string EmailSort { get; set; }

        // Filtering
        public string CurrentFilter { get; set; }

        // Paging
        public PaginatedList<Customer> Customers { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            EmailSort = sortOrder == "Email" ? "email_desc" : "Email";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Customer> customerIQ = from c in _context.Customers
                                              select c;

            // Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                customerIQ = customerIQ.Where(c => c.LastName.Contains(searchString)
                                                || c.FirstName.Contains(searchString)
                                                || c.Email.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    customerIQ = customerIQ.OrderByDescending(c => c.LastName);
                    break;
                case "Email":
                    customerIQ = customerIQ.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    customerIQ = customerIQ.OrderByDescending(c => c.Email);
                    break;
                default:
                    customerIQ = customerIQ.OrderBy(c => c.LastName);
                    break;
            }

            // Paging
            int pageSize = 5;
            Customers = await PaginatedList<Customer>.CreateAsync(
                customerIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
