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
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _categoryRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _categoryRepoMock = new Mock<ICategoryRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new CategoryService(_categoryRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WithoutName_ThrowsArgumentException()
        {
            var categoryDto = new CategoryDto { Name = "", Description = "desc" };

            var act = async () => await _service.CreateAsync(categoryDto);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*name*");
        }

        [Fact]
        public async Task CreateAsync_WithValidCategory_ReturnsCategory()
        {
            var categoryDto = new CategoryDto { Name = "Networking", Description = "desc" };
            var mappedCategory = new Category { Name = "Networking", Description = "desc" };

            _mapperMock.Setup(m => m.Map<Category>(categoryDto)).Returns(mappedCategory);
            _categoryRepoMock.Setup(r => r.AddAsync(mappedCategory)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CategoryDto>(mappedCategory)).Returns(categoryDto);

            var result = await _service.CreateAsync(categoryDto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Networking");
            _categoryRepoMock.Verify(r => r.AddAsync(mappedCategory), Times.Once);
        }
    }
}
