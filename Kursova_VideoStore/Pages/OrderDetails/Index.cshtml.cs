using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Videoteka.Models;

namespace Kursova_VideoStore.Pages.OrderDetails
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
        public string FilmSort { get; set; }
        public string QuantitySort { get; set; }
        public string OrderSort { get; set; }

        // Filtering
        public string CurrentFilter { get; set; }

        // Paging
        public PaginatedList<OrderDetail> OrderDetails { get; set; } = default!;

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            CurrentSort = sortOrder;
            FilmSort = String.IsNullOrEmpty(sortOrder) ? "film_desc" : "";
            QuantitySort = sortOrder == "Quantity" ? "qty_desc" : "Quantity";
            OrderSort = sortOrder == "Order" ? "order_desc" : "Order";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            IQueryable<OrderDetail> orderDetailIQ = _context.OrderDetails
                .Include(o => o.Film)
                .Include(o => o.Order);

            // Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                orderDetailIQ = orderDetailIQ.Where(od =>
                    od.Film.Title.Contains(searchString) ||
                    od.Order.Customer.LastName.Contains(searchString));
            }

            // Sorting
            switch (sortOrder)
            {
                case "film_desc":
                    orderDetailIQ = orderDetailIQ.OrderByDescending(od => od.Film.Title);
                    break;
                case "Quantity":
                    orderDetailIQ = orderDetailIQ.OrderBy(od => od.Quantity);
                    break;
                case "qty_desc":
                    orderDetailIQ = orderDetailIQ.OrderByDescending(od => od.Quantity);
                    break;
                case "Order":
                    orderDetailIQ = orderDetailIQ.OrderBy(od => od.Order.OrderDate);
                    break;
                case "order_desc":
                    orderDetailIQ = orderDetailIQ.OrderByDescending(od => od.Order.OrderDate);
                    break;
                default:
                    orderDetailIQ = orderDetailIQ.OrderBy(od => od.Film.Title);
                    break;
            }

            // Paging
            int pageSize = 5;
            OrderDetails = await PaginatedList<OrderDetail>.CreateAsync(
                orderDetailIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
