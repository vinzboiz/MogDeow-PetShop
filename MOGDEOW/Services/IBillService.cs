using MOGDEOW.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public interface IBillService
    {
        Task<IEnumerable<Bill>> GetAllBillsAsync();
        Task<Bill> GetBillByIdAsync(int id);
        Task<Bill> AddBillAsync(Bill bill);
        Task<bool> UpdateBillAsync(Bill bill);
        Task<bool> DeleteBillAsync(int id);
        Task<IEnumerable<Bill>> GetTodayOrdersAsync();
        Task<IEnumerable<Bill>> GetBillsByUserIdAsync(int userId);

        Task<IEnumerable<Bill>> GetOrdersByDateAsync(DateTime date);
        Task<List<BillDetail>> GetBillDetailsByBillIdAsync(int billId);
        Task<(decimal totalRevenue, int totalOrders, List<string> dates, List<decimal> values)> GetRevenueStatsAsync();
        Task<(decimal totalRevenue, int totalOrders, List<string> revenueDates, List<decimal> revenueValues)> GetRevenueStatsAsync(DateTime? fromDate = null, DateTime? toDate = null, string topRange = null, int? selectedTypeId = null);

    }
}
