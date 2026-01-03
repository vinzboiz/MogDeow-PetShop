using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Serialization;

namespace MOGDEOW.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }

        [Required, MaxLength(255), Column(TypeName = "NVARCHAR")]
        public string ProductName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ProductPrice { get; set; }

        [Required, Column(TypeName = "NVARCHAR(MAX)")]
        public string ProductImage { get; set; }

        [Required]
        public string ProductDetail { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required, MaxLength(50)]
        public string DVT { get; set; }
        [Required]
        public int ProductTypeID { get; set; }
        [ForeignKey(nameof(ProductTypeID))]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] 
        public virtual ProductType ProductType { get; set; }
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
        public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
        [NotMapped]
        public IFormFile Upload { get; set; }
        [NotMapped]
        public string SelectedTypeName { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}
