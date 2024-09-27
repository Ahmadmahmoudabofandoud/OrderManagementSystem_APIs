using OrderManagementSystem.Core;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> CreateNewProductAsync(Product product)
        {
             _unitOfWork.Repository<Product>().Add(product);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;

            return product;
        }

        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();
            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.CompleteAsync();
        }
    }
}
