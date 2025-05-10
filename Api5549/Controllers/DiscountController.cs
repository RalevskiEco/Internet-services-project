using App5549.DTOs;
using App5549.Interfaces;
using Microsoft.AspNetCore.Mvc;
using App5549.Interfaces;


namespace Api5549.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _service;

        public DiscountController(IDiscountService service)
        {
            _service = service;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateDiscount([FromBody] List<BasketItemDto> basket)
        {
            try
            {
                var result = await _service.CalculateDiscountAsync(basket);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
