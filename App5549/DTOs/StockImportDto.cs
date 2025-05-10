using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App5549.DTOs
{
    public class StockImportDto
    {
        public string Name { get; set; } = null!;
        public List<string> Categories { get; set; } = new();
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
