using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Employees
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
        public string FirstNameSort { get; set; }
        public string LastNameSort { get; set; }
        public string PositionSort { get; set; }
        public string HireDateSort { get; set; }

        // Filtering
        public string CurrentFilter { get; set; }

        // Paging
        public PaginatedList<Employee> Employees { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            FirstNameSort = String.IsNullOrEmpty(sortOrder) ? "first_desc" : "";
            LastNameSort = sortOrder == "LastName" ? "last_desc" : "LastName";
            PositionSort = sortOrder == "Position" ? "pos_desc" : "Position";
            HireDateSort = sortOrder == "HireDate" ? "date_desc" : "HireDate";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Employee> employeeIQ = from e in _context.Employees
                                              select e;

            // Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                employeeIQ = employeeIQ.Where(e => e.LastName.Contains(searchString)
                                                || e.FirstName.Contains(searchString)
                                                || e.Position.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "first_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.FirstName);
                    break;
                case "LastName":
                    employeeIQ = employeeIQ.OrderBy(e => e.LastName);
                    break;
                case "last_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.LastName);
                    break;
                case "Position":
                    employeeIQ = employeeIQ.OrderBy(e => e.Position);
                    break;
                case "pos_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.Position);
                    break;
                case "HireDate":
                    employeeIQ = employeeIQ.OrderBy(e => e.HireDate);
                    break;
                case "date_desc":
                    employeeIQ = employeeIQ.OrderByDescending(e => e.HireDate);
                    break;
                default:
                    employeeIQ = employeeIQ.OrderBy(e => e.FirstName);
                    break;
            }

            // Paging
            int pageSize = 5;
            Employees = await PaginatedList<Employee>.CreateAsync(
                employeeIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
