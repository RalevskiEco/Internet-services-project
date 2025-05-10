using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App5549.DTOs;

namespace App5549.Interfaces
{
    public interface IStockImportService
    {
        Task ImportAsync(IEnumerable<StockImportDto> stockList);
    }
}
