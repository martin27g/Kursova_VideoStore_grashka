using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Videoteka.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        [Display(Name = "Customer")]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        [Display(Name = "Employee")]
        public int EmployeeID { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Order date is required.")]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        public Customer? Customer { get; set; }
        public Employee? Employee { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
