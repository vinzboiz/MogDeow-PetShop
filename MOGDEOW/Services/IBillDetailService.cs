using MOGDEOW.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOGDEOW.Services
{
    public interface IBillDetailService
    {
        Task<IEnumerable<BillDetail>> GetBillDetailsByBillIdAsync(int billId);
        Task<BillDetail> AddBillDetailAsync(BillDetail billDetail);
    }
}
