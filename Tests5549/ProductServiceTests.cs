using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using FluentAssertions;
using App5549.Services;
using App5549.DTOs;
using Domain5549.Entities;
using Domain5549.Interfaces;

namespace Tests5549
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new ProductService(_productRepoMock.Object, _categoryRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenNameMissing()
        {
            var productDto = new ProductDto
            {
                Name = "",
                Price = 100,
                Categories = new List<string> { "CPU" }
            };

            var act = async () => await _service.CreateAsync(productDto);

            await act.Should().ThrowAsync<System.ArgumentException>()
                .WithMessage("*name*");
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenPriceInvalid()
        {
            var productDto = new ProductDto
            {
                Name = "Test Product",
                Price = 0,
                Categories = new List<string> { "CPU" }
            };

            var act = async () => await _service.CreateAsync(productDto);

            await act.Should().ThrowAsync<System.ArgumentException>()
                .WithMessage("*price*");
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenNoCategories()
        {
            var productDto = new ProductDto
            {
                Name = "Test Product",
                Price = 100,
                Categories = new List<string>()
            };

            var act = async () => await _service.CreateAsync(productDto);

            await act.Should().ThrowAsync<System.ArgumentException>()
                .WithMessage("*category*");
        }

        [Fact]
        public async Task CreateAsync_Success()
        {
            var productDto = new ProductDto
            {
                Name = "Test Product",
                Price = 100,
                Categories = new List<string> { "CPU" }
            };

            var mappedProduct = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Categories = new List<Category>()
            };

            var cpuCategory = new Category { Name = "CPU" };

            _mapperMock.Setup(m => m.Map<Product>(productDto)).Returns(mappedProduct);
            _categoryRepoMock.Setup(c => c.GetAllAsync()).ReturnsAsync(new List<Category> { cpuCategory });
            _productRepoMock.Setup(p => p.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ProductDto>(It.IsAny<Product>())).Returns(productDto);

            var result = await _service.CreateAsync(productDto);

            result.Should().NotBeNull();
            mappedProduct.Categories.Should().Contain(cpuCategory);
            _productRepoMock.Verify(p => p.AddAsync(mappedProduct), Times.Once);
        }
    }
}
