using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOGDEOW.Models
{
    public class BillDetail
    {
        [Key]
        public int BillDetailID { get; set; }

        [Required]
        public int BillID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Price { get; set; }

        [ForeignKey(nameof(BillID))]
        public Bill Bill { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }
    }

}
