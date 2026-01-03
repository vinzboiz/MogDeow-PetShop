using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MOGDEOW.Models
{
    public class ProductType
    {
        [Key]
        public int ProductTypeID { get; set; }
        [Required, MaxLength(100), Column(TypeName = "NVARCHAR")]
        public string ProductTypeName { get; set; }
        [Required, MaxLength(100)]
        public string Allocate { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
