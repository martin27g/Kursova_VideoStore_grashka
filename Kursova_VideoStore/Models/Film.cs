using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Videoteka.Models
{
    public class Film
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FilmID { get; set; }

        [Required(ErrorMessage = "Film title is required.")]
        [StringLength(100, ErrorMessage = "Film title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
        public string? Genre { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Release Year")]
        public int ReleaseYear { get; set; }

        [Range(0, 999.99, ErrorMessage = "Price must be between 0 and 999.99.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number.")]
        public int Stock { get; set; }

      
        public bool IsActive { get; set; } = true;

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}