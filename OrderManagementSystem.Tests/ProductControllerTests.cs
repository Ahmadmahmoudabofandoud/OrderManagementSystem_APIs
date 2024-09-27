using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagementSystem.APIs.Controllers;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ProductController _controller;
        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockUserService = new Mock<IUserService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new ProductController(_mockProductService.Object, _mockUserService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithListOfProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product1", Price = 100, Stock = 10 },
                new Product { ProductId = 2, ProductName = "Product2", Price = 200, Stock = 20 }
            };
            var productDtos = new List<ProductToReturnDto>
            {
                new ProductToReturnDto { ProductId = 1, ProductName = "Product1", Price = 100, Stock = 10 },
                new ProductToReturnDto { ProductId = 2, ProductName = "Product2", Price = 200, Stock = 20 }
            };

            _mockProductService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(products);
            _mockMapper.Setup(m => m.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products)).Returns(productDtos);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<ProductToReturnDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetProductById_ReturnsOkResult_WithProduct()
        {
            // Arrange
            int productId = 1;
            var product = new Product { ProductId = productId, ProductName = "Product1", Price = 100, Stock = 10 };
            var productDto = new ProductToReturnDto { ProductId = productId, ProductName = "Product1", Price = 100, Stock = 10 };

            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(m => m.Map<Product, ProductToReturnDto>(product)).Returns(productDto);

            // Act
            var result = await _controller.GetProductById(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<ProductToReturnDto>(okResult.Value);
            Assert.Equal(productId, returnValue.ProductId);
        }

        [Fact]
        public async Task CreateProduct_ReturnsBadRequest_WhenProductDtoIsNull()
        {
            // Act
            var result = await _controller.CreateProduct(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(400, apiResponse.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsOkResult_WithCreatedProduct()
        {
            // Arrange
            var productDto = new ProductDto { ProductName = "Product1", Price = 100, Stock = 10 };
            var product = new Product { ProductId = 1, ProductName = productDto.ProductName, Price = productDto.Price, Stock = productDto.Stock };

            _mockProductService.Setup(service => service.CreateNewProductAsync(It.IsAny<Product>())).ReturnsAsync(product);

            // Act
            var result = await _controller.CreateProduct(productDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productDto.ProductName, returnValue.ProductName);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenProductDtoIsNull()
        {
            // Act
            var result = await _controller.UpdateProduct(1, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(400, apiResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            var productDto = new ProductDto { ProductName = "Product1", Price = 100, Stock = 10 };

            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(productId, productDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsOkResult_WhenProductIsUpdated()
        {
            // Arrange
            int productId = 1;
            var productDto = new ProductDto { ProductName = "UpdatedProduct", Price = 150, Stock = 15 };
            var existingProduct = new Product { ProductId = productId, ProductName = "Product1", Price = 100, Stock = 10 };

            _mockProductService.Setup(service => service.GetProductByIdAsync(productId)).ReturnsAsync(existingProduct);
            _mockProductService.Setup(service => service.UpdateProductAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateProduct(productId, productDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Order Data Updated Successfully", returnValue);
        }
    }
}
