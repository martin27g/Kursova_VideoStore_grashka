using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Videoteka.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }

        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [StringLength(50, ErrorMessage = "Position cannot exceed 50 characters.")]
        [Display(Name = "Position")]
        public string? Position { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        [Required(ErrorMessage = "Hire date is required.")]
        public DateTime HireDate { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
