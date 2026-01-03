using MOGDEOW.Data;
using MOGDEOW.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public class BillService : IBillService
    {
        private readonly MogdeowContext _context;

        public BillService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bill>> GetAllBillsAsync()
        {
            return await _context.Bills
                .Include(b => b.User)
                .Include(b => b.BillDetails)
                .ToListAsync();
        }

        public async Task<Bill> GetBillByIdAsync(int id)
        {
            return await _context.Bills
                .Include(b => b.User)
                .Include(b => b.BillDetails)
                .FirstOrDefaultAsync(b => b.BillID == id);
        }
        public async Task<Bill> AddBillAsync(Bill bill)
        {
            // Không để Date bị null
            if (bill.Date == default)
            {
                bill.Date = DateTime.Now;
            }

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();
            return bill;
        }


        public async Task<bool> UpdateBillAsync(Bill bill)
        {
            _context.Bills.Update(bill);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBillAsync(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return false;

            _context.Bills.Remove(bill);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<IEnumerable<Bill>> GetTodayOrdersAsync()
        {
            var today = DateTime.Today;
            return await _context.Bills
                .Include(b => b.User)
                .Include(b => b.BillDetails)
                .Where(b => b.Date.Date == today)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetBillsByUserIdAsync(int userId)
        {
            return await _context.Bills
                .Where(b => b.UserID == userId)
                .Include(b => b.User)
                .Include(b => b.BillDetails)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bill>> GetOrdersByDateAsync(DateTime date)
        {
             return await _context.Bills
                .Include(b => b.User)
                .Where(b => b.Date.Date == date.Date)
                .ToListAsync();
        }

        public async Task<List<BillDetail>> GetBillDetailsByBillIdAsync(int billId)
        {
            return await _context.BillDetails
                .Where(bd => bd.BillID == billId)
                .Include(bd => bd.Product) 
                .ToListAsync();
        }
        public async Task<(decimal totalRevenue, int totalOrders, List<string> dates, List<decimal> values)> GetRevenueStatsAsync()
        {
            var bills = await _context.Bills
                .Where(b => b.Status == "Đã thanh toán")
                .ToListAsync();

            decimal total = bills.Sum(b => b.Total); 
            int count = bills.Count; 

            var revenueByDate = bills
                .GroupBy(b => b.Date.Date) 
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key.ToString("dd/MM"), g => g.Sum(b => b.Total)); 

            return (total, count, revenueByDate.Keys.ToList(), revenueByDate.Values.ToList());
        }

        public async Task<(decimal totalRevenue, int totalOrders, List<string> revenueDates, List<decimal> revenueValues)>
GetRevenueStatsAsync(DateTime? fromDate = null, DateTime? toDate = null, string topRange = null, int? selectedTypeId = null)
        {
            var query = _context.Bills
                .Include(b => b.BillDetails).ThenInclude(d => d.Product)
                .Where(b => b.Status == "Đã thanh toán");

            // Lọc theo ngày nếu có
            if (fromDate.HasValue && toDate.HasValue)
            {
                query = query.Where(b => b.Date.Date >= fromDate.Value.Date && b.Date.Date <= toDate.Value.Date);
            }
            else if (!string.IsNullOrEmpty(topRange))
            {
                if (topRange == "today")
                {
                    var today = DateTime.Today;
                    query = query.Where(b => b.Date.Date == today);
                }
                else if (topRange == "month")
                {
                    var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    query = query.Where(b => b.Date.Date >= startOfMonth && b.Date.Date <= DateTime.Today);
                }
                else if (topRange == "year")
                {
                    var startOfYear = new DateTime(DateTime.Today.Year, 1, 1);
                    query = query.Where(b => b.Date.Date >= startOfYear && b.Date.Date <= DateTime.Today);
                }
            }

            // Nếu lọc theo loại sản phẩm
            if (selectedTypeId.HasValue)
            {
                query = query.Where(b => b.BillDetails.Any(d => d.Product.ProductTypeID == selectedTypeId.Value));
            }

            var filteredBills = await query.ToListAsync();

            // Nhóm doanh thu theo ngày
            var grouped = filteredBills
                .GroupBy(b => b.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd/MM"),
                    Total = g.Sum(b => b.Total)
                }).ToList();

            decimal totalRevenue = filteredBills.Sum(b => b.Total);
            int totalOrders = filteredBills.Count;

            var revenueDates = grouped.Select(g => g.Date).ToList();
            var revenueValues = grouped.Select(g => g.Total).ToList();

            return (totalRevenue, totalOrders, revenueDates, revenueValues);
        }

    }
}
