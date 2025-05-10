using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App5549.DTOs;
using App5549.Interfaces;
using AutoMapper;
using Domain5549.Entities;
using Domain5549.Interfaces;

namespace App5549.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, ICategoryRepository categoryRepo, IMapper mapper)
        {
            _repo = repo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(ProductDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required.");
            if (dto.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero.");
            if (dto.Categories == null || !dto.Categories.Any())
                throw new ArgumentException("At least one category is required.");

            var product = _mapper.Map<Product>(dto);

            product.Categories.Clear();
            var allCategories = await _categoryRepo.GetAllAsync();
            foreach (var catName in dto.Categories)
            {
                var category = allCategories.FirstOrDefault(c => c.Name == catName);
                if (category != null)
                    product.Categories.Add(category);
            }

            await _repo.AddAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> UpdateAsync(int id, ProductDto dto)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Product name is required.");
            if (dto.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero.");
            if (dto.Categories == null || !dto.Categories.Any())
                throw new ArgumentException("At least one category is required.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;

            product.Categories.Clear();
            var allCategories = await _categoryRepo.GetAllAsync();
            foreach (var catName in dto.Categories)
            {
                var category = allCategories.FirstOrDefault(c => c.Name == catName);
                if (category != null)
                    product.Categories.Add(category);
            }

            await _repo.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exists = await _repo.ExistsAsync(id);
            if (!exists) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }
}
