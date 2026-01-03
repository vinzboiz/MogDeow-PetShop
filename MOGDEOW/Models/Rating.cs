using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOGDEOW.Models
{
    public class Rating
    {
        [Key]
        public int RateID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int Rate { get; set; }

        [Required, MaxLength(500)]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Message { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(UserID))]
        public User User { get; set; }

        [ForeignKey(nameof(ProductID))]
        public Product Product { get; set; }
    }

}
