using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using App5549.DTOs;

namespace App5549.Interfaces
{
    public interface IDiscountService
    {
        Task<DiscountResultDto> CalculateDiscountAsync(IEnumerable<BasketItemDto> basket);
    }
}
