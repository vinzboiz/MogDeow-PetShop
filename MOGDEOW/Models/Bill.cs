using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MOGDEOW.Models
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }
        public string ReceiverName { get; set; }     
        public string ReceiverPhone { get; set; }    
        public string ShippingAddress { get; set; }  

        [Required]
        public int UserID { get; set; }

        // Quan hệ với User
        [ForeignKey("UserID")]
        public User User { get; set; }

        // Quan hệ với BillDetail
        public ICollection<BillDetail> BillDetails { get; set; }
    }
}
