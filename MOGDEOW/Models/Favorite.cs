using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MOGDEOW.Models
{
    public class Favorite
    {
        [Key, Column(Order = 0)]
        public int ProductID { get; set; }

        [Key, Column(Order = 1)]
        public int UserID { get; set; }
        [ForeignKey(nameof(ProductID))]
        [JsonIgnore]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(UserID))]
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
