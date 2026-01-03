using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOGDEOW.Models
{
    public class CartDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartDetailID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime DateIn { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }
    }


}