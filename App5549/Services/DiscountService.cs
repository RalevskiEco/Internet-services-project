using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using App5549.DTOs;
using App5549.Interfaces;
using Domain5549.Interfaces;

namespace App5549.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IProductRepository _productRepository;

        public DiscountService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<DiscountResultDto> CalculateDiscountAsync(IEnumerable<BasketItemDto> basket)
        {
            var basketList = basket.ToList();
            if (!basketList.Any())
                return new DiscountResultDto
                {
                    TotalPrice = 0,
                    Discount = 0,
                    FinalPrice = 0,
                    Message = "Basket is empty."
                };

            var productIds = basketList.Select(b => b.ProductId).ToList();

            var products = await _productRepository.GetProductsWithCategoriesAsync(productIds);

            if (products == null || !products.Any())
            {
                return new DiscountResultDto
                {
                    TotalPrice = 0,
                    Discount = 0,
                    FinalPrice = 0,
                    Message = "No products found for the given IDs."
                };
            }

            decimal totalPrice = 0;
            decimal discount = 0;
            string message = "No discount applied.";

            var categoryProductMap = new Dictionary<string, List<BasketItemDto>>();

            foreach (var item in basketList)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                    return new DiscountResultDto
                    {
                        TotalPrice = 0,
                        Discount = 0,
                        FinalPrice = 0,
                        Message = $"Product with ID {item.ProductId} not found."
                    };

                if (item.Quantity > product.Quantity)
                    return new DiscountResultDto
                    {
                        TotalPrice = 0,
                        Discount = 0,
                        FinalPrice = 0,
                        Message = $"Not enough stock for product {product.Name}."
                    };

                totalPrice += product.Price * item.Quantity;

                foreach (var category in product.Categories)
                {
                    var categoryName = category.Name;
                    if (!categoryProductMap.ContainsKey(categoryName))
                        categoryProductMap[categoryName] = new List<BasketItemDto>();
                    categoryProductMap[categoryName].Add(item);
                }

            }

            foreach (var kvp in categoryProductMap)
            {
                var itemsInCategory = kvp.Value;
                if (itemsInCategory.Sum(i => i.Quantity) > 1)
                {
                    foreach (var item in itemsInCategory)
                    {
                        var product = products.First(p => p.Id == item.ProductId);
                        if (item.Quantity > 1)
                        {
                            discount += product.Price * 0.05m;
                            message = "5% discount applied to the first copy of eligible products.";
                        }
                    }
                }
            }

            return new DiscountResultDto
            {
                TotalPrice = Math.Round(totalPrice, 2),
                Discount = Math.Round(discount, 2),
                FinalPrice = Math.Round(totalPrice - discount, 2),
                Message = message
            };
        }

    }
}
