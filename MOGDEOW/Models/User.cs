using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MOGDEOW.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [MaxLength(100), Column(TypeName = "NVARCHAR")]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(255), Column(TypeName = "NVARCHAR")]
        public string Address { get; set; } = "Chưa cập nhật"; 

        [MaxLength(15)]
        public string PhoneNumber { get; set; } = "Chưa có số"; 

        [MaxLength(50)]
        public string Role { get; set; } = "Customer"; 

        [Column(TypeName = "VARBINARY(MAX)")]
        public string UserImg { get; set; } = "";

        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    }
}