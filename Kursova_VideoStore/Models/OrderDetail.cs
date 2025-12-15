using System;
using System.ComponentModel.DataAnnotations;

namespace Videoteka.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        public int OrderID { get; set; }
        public Order? Order { get; set; }

        public int FilmID { get; set; }
        public Film? Film { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        // NEW: When the movie must be back
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        // NEW: When it was actually returned (Null = Still Rented)
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }
    }
}