using Kursova_VideoStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Videoteka.Models;

namespace Kursova_VideoStore.Controllers
{
    [Authorize(Roles = Roles.AdminEndUser)]
    public class OrderDetailsController : Controller
    {
        private readonly VideotekaContext _context;

        public OrderDetailsController(VideotekaContext context)
        {
            _context = context;
        }

        // взимане
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["OrderSortParm"] = String.IsNullOrEmpty(sortOrder) ? "order_desc" : "";
            ViewData["FilmSortParm"] = sortOrder == "Film" ? "film_desc" : "Film";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            if (searchString != null) pageNumber = 1;
            else searchString = currentFilter;

            ViewData["CurrentFilter"] = searchString;

            var orderDetails = _context.OrderDetails
                .Include(o => o.Film)
                .Include(o => o.Order)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                if (int.TryParse(searchString, out int orderIdSearch))
                {
                    orderDetails = orderDetails.Where(s => s.OrderID == orderIdSearch
                                                        || s.Film.Title.Contains(searchString));
                }
                else
                {
                    orderDetails = orderDetails.Where(s => s.Film.Title.Contains(searchString));
                }
            }

            switch (sortOrder)
            {
                case "order_desc":
                    orderDetails = orderDetails.OrderByDescending(s => s.OrderID);
                    break;
                case "Film":
                    orderDetails = orderDetails.OrderBy(s => s.Film.Title);
                    break;
                case "film_desc":
                    orderDetails = orderDetails.OrderByDescending(s => s.Film.Title);
                    break;
                case "Status":
                    orderDetails = orderDetails.OrderBy(s => s.ReturnDate);
                    break;
                default:
                    orderDetails = orderDetails.OrderBy(s => s.OrderID);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<OrderDetail>.CreateAsync(orderDetails.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // Маркиране като върнат
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnFilm(int id)
        {
            var item = await _context.OrderDetails.FindAsync(id);
            if (item != null && item.ReturnDate == null)
            {
                item.ReturnDate = DateTime.Now;
                _context.Update(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var orderDetail = await _context.OrderDetails
                .Include(o => o.Film)
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null) return NotFound();
            return View(orderDetail);
        }


        public IActionResult Create()
        {
            // FIX: Only show films that are Active
            var activeFilms = _context.Films.Where(f => f.IsActive);

            ViewData["FilmID"] = new SelectList(activeFilms, "FilmID", "Title");
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailID,OrderID,FilmID,Quantity")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                orderDetail.DueDate = DateTime.Now.AddDays(7);
                orderDetail.ReturnDate = null;

                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // If failed, reload list (filtered)
            ViewData["FilmID"] = new SelectList(_context.Films.Where(f => f.IsActive), "FilmID", "Title", orderDetail.FilmID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }

        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null) return NotFound();

            // Note: For EDIT, we generally show ALL films (even inactive ones) 
            // in case you are editing an old historical order. 
            // However, if you want to force them to switch to an active film, use the .Where() clause here too.
            // For now, I will leave it showing all films to preserve history editing.
            ViewData["FilmID"] = new SelectList(_context.Films, "FilmID", "Title", orderDetail.FilmID);

            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID,OrderID,FilmID,Quantity,DueDate,ReturnDate")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailID) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(orderDetail); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!OrderDetailExists(orderDetail.OrderDetailID)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FilmID"] = new SelectList(_context.Films, "FilmID", "Title", orderDetail.FilmID);
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", orderDetail.OrderID);
            return View(orderDetail);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var orderDetail = await _context.OrderDetails.Include(o => o.Film).Include(o => o.Order).FirstOrDefaultAsync(m => m.OrderDetailID == id);
            if (orderDetail == null) return NotFound();
            return View(orderDetail);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null) _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderDetailExists(int id) => _context.OrderDetails.Any(e => e.OrderDetailID == id);
    }
}