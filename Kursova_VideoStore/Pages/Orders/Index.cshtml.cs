using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.Orders
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
        public string DateSort { get; set; }
        public string CustomerSort { get; set; }
        public string EmployeeSort { get; set; }

        // Filtering
        public string CurrentFilter { get; set; }

        // Paging
        public PaginatedList<Order> Orders { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            DateSort = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            CustomerSort = sortOrder == "Customer" ? "cust_desc" : "Customer";
            EmployeeSort = sortOrder == "Employee" ? "emp_desc" : "Employee";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<Order> orderIQ = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee);

            // Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                orderIQ = orderIQ.Where(o =>
                    o.Customer.LastName.Contains(searchString) ||
                    o.Customer.FirstName.Contains(searchString) ||
                    o.Employee.LastName.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "date_desc":
                    orderIQ = orderIQ.OrderByDescending(o => o.OrderDate);
                    break;
                case "Customer":
                    orderIQ = orderIQ.OrderBy(o => o.Customer.LastName);
                    break;
                case "cust_desc":
                    orderIQ = orderIQ.OrderByDescending(o => o.Customer.LastName);
                    break;
                case "Employee":
                    orderIQ = orderIQ.OrderBy(o => o.Employee.LastName);
                    break;
                case "emp_desc":
                    orderIQ = orderIQ.OrderByDescending(o => o.Employee.LastName);
                    break;
                default:
                    orderIQ = orderIQ.OrderBy(o => o.OrderDate);
                    break;
            }

            // Paging
            int pageSize = 5;
            Orders = await PaginatedList<Order>.CreateAsync(
                orderIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
