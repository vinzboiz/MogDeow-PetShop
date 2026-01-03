using System;
using System.Collections.Generic;
using MOGDEOW.Models;

namespace MOGDEOW.Models.ViewModels
{
    public class AccountViewModel
    {
        public User User { get; set; }
        public IEnumerable<Bill> Bills { get; set; }
        public IEnumerable<Bill> TodayOrders { get; set; }
        public string SelectedSection { get; set; }
        public DateTime SelectedDate { get; set; }
        public List<BillDetail> BillDetails { get; set; } 
        public int? SelectedBillID { get; set; }
        public IEnumerable<Product>? ProductStats { get; set; }    
        public List<ProductType> ProductTypes { get; set; }        
        public int? SelectedTypeId { get; set; }                   
        public decimal TotalRevenue { get; set; }
        public int TotalCompletedOrders { get; set; }
        public List<string> RevenueDates { get; set; } = new();
        public List<decimal> RevenueValues { get; set; } = new();
        public Bill Bill { get; set; }
    }
}
