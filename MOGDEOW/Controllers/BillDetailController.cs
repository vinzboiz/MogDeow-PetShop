using MOGDEOW.Models;
using MOGDEOW.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOGDEOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillDetailController : ControllerBase
    {
        private readonly IBillDetailService _billDetailService;

        public BillDetailController(IBillDetailService billDetailService)
        {
            _billDetailService = billDetailService;
        }

        [HttpGet("{billId}")]
        public async Task<ActionResult<IEnumerable<BillDetail>>> GetBillDetails(int billId)
        {
            return Ok(await _billDetailService.GetBillDetailsByBillIdAsync(billId));
        }

        [HttpPost]
        public async Task<ActionResult<BillDetail>> AddBillDetail(BillDetail billDetail)
        {
            var newBillDetail = await _billDetailService.AddBillDetailAsync(billDetail);
            return CreatedAtAction(nameof(GetBillDetails), new { billId = newBillDetail.BillID }, newBillDetail);
        }
    }
}
