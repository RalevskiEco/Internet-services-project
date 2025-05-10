using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain5549.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain5549.Entities;


namespace Domain5549.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<Product>> GetProductsWithCategoriesAsync(List<int> productIds);




    }
}
