using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App5549.DTOs;
using App5549.Interfaces;
using AutoMapper;
using Domain5549.Entities;
using Domain5549.Interfaces;

namespace App5549.Services
{
    public class StockImportService : IStockImportService
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public StockImportService(IProductRepository productRepo, ICategoryRepository categoryRepo, IMapper mapper)
        {
            _productRepo = productRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task ImportAsync(IEnumerable<StockImportDto> stockList)
        {
            foreach (var stock in stockList)
            {
                var categories = new List<Category>();
                foreach (var catName in stock.Categories)
                {
                    var category = (await _categoryRepo.GetAllAsync()).FirstOrDefault(c => c.Name == catName.Trim());
                    if (category == null)
                    {
                        category = new Category { Name = catName.Trim() };
                        await _categoryRepo.AddAsync(category);
                    }
                    categories.Add(category);
                }

                var product = (await _productRepo.GetAllAsync()).FirstOrDefault(p => p.Name == stock.Name.Trim());
                if (product == null)
                {
                    product = new Product
                    {
                        Name = stock.Name.Trim(),
                        Price = stock.Price,
                        Quantity = stock.Quantity,
                        Categories = categories
                    };
                    await _productRepo.AddAsync(product);
                }
                else
                {
                    product.Quantity += stock.Quantity;
                    foreach (var cat in categories)
                    {
                        if (!product.Categories.Any(c => c.Id == cat.Id))
                        {
                            product.Categories.Add(cat);
                        }
                    }
                    await _productRepo.UpdateAsync(product);
                }
            }
        }
    }
}

