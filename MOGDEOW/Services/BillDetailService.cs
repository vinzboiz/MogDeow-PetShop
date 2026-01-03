using MOGDEOW.Data;
using MOGDEOW.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public class BillDetailService : IBillDetailService
    {
        private readonly MogdeowContext _context;

        public BillDetailService(MogdeowContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BillDetail>> GetBillDetailsByBillIdAsync(int billId)
        {
            return await _context.BillDetails
                .Where(bd => bd.BillID == billId)
                .Include(bd => bd.Product)
                .ToListAsync();
        }

        public async Task<BillDetail> AddBillDetailAsync(BillDetail billDetail)
        {
            _context.BillDetails.Add(billDetail);
            await _context.SaveChangesAsync();
            return billDetail;
        }
    }
}
