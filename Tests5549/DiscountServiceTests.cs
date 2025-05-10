using Xunit;
using Moq;
using App5549.Services;
using App5549.DTOs;
using Domain5549.Entities;
using Domain5549.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;

namespace Tests5549
{
    public class DiscountServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly DiscountService _service;

        public DiscountServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _service = new DiscountService(_productRepoMock.Object);
        }

        [Fact]
        public async Task SingleProduct_NoDiscount()
        {
            var basket = new List<BasketItemDto>
            {
                new BasketItemDto { ProductId = 1, Quantity = 1 }
            };

            _productRepoMock.Setup(r => r.GetProductsWithCategoriesAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Intel's Core i9-9900K",
                        Price = 475.99m,
                        Quantity = 2,
                        Categories = new List<Category> { new Category { Name = "CPU" } }
                    }
                });

            var result = await _service.CalculateDiscountAsync(basket);

            result.TotalPrice.Should().Be(475.99m);
            result.Discount.Should().Be(0);
            result.FinalPrice.Should().Be(475.99m);
            result.Message.Should().Contain("No discount");
        }

        [Fact]
        public async Task MultipleSameCategory_ApplyDiscount()
        {
            var basket = new List<BasketItemDto>
            {
                new BasketItemDto { ProductId = 1, Quantity = 2 }
            };

            _productRepoMock.Setup(r => r.GetProductsWithCategoriesAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Intel's Core i9-9900K",
                        Price = 475.99m,
                        Quantity = 5,
                        Categories = new List<Category> { new Category { Name = "CPU" } }
                    }
                });

            var result = await _service.CalculateDiscountAsync(basket);

            result.TotalPrice.Should().Be(951.98m);
            result.Discount.Should().BeApproximately(23.80m, 0.01m); // 5% of 475.99
            result.FinalPrice.Should().BeApproximately(928.18m, 0.01m);
            result.Message.Should().Contain("5% discount");
        }

        [Fact]
        public async Task NotEnoughStock_ReturnsError()
        {
            var basket = new List<BasketItemDto>
            {
                new BasketItemDto { ProductId = 1, Quantity = 10 }
            };

            _productRepoMock.Setup(r => r.GetProductsWithCategoriesAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Intel's Core i9-9900K",
                        Price = 475.99m,
                        Quantity = 2,
                        Categories = new List<Category> { new Category { Name = "CPU" } }
                    }
                });

            var result = await _service.CalculateDiscountAsync(basket);

            result.Message.Should().Contain("Not enough stock");
            result.TotalPrice.Should().Be(0);
            result.Discount.Should().Be(0);
            result.FinalPrice.Should().Be(0);
        }
    }
}
