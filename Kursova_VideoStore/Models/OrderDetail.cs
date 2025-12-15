using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Videoteka.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }

        [Required(ErrorMessage = "Order is required.")]
        [Display(Name = "Order")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Film is required.")]
        [Display(Name = "Film")]
        public int FilmID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        public Order? Order { get; set; }
        public Film? Film { get; set; }
    }
}
