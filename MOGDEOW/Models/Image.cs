using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOGDEOW.Models
{
    public class Image
    {
        [Key]
        public int ImageID { get; set; }

        [Required, MaxLength(255)]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string LinkImage { get; set; }

        [Required]
        public int ProductID { get; set; }

        [ForeignKey("ProductID")]
        public Product Product { get; set; }
    }
}