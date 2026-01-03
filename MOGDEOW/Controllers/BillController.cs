using MOGDEOW.Models;
using MOGDEOW.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MOGDEOW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetAllBills()
        {
            return Ok(await _billService.GetAllBillsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBillById(int id)
        {
            var bill = await _billService.GetBillByIdAsync(id);
            if (bill == null) return NotFound();
            return Ok(bill);
        }

        [HttpPost]
        public async Task<ActionResult<Bill>> AddBill(Bill bill)
        {
            var newBill = await _billService.AddBillAsync(bill);
            return CreatedAtAction(nameof(GetBillById), new { id = newBill.BillID }, newBill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBill(int id, Bill bill)
        {
            if (id != bill.BillID) return BadRequest();
            var updated = await _billService.UpdateBillAsync(bill);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var deleted = await _billService.DeleteBillAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
