using App5549.DTOs;
using App5549.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api5549.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockImportService _service;

        public StockController(IStockImportService service)
        {
            _service = service;
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] List<StockImportDto> stockList)
        {
            if (stockList == null || !stockList.Any())
                return BadRequest(new { Message = "Stock list is required." });

            await _service.ImportAsync(stockList);
            return Ok(new { Message = "Stock imported successfully." });
        }
    }
}