    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using OrderManagementSystem.APIs.DTOs;
    using OrderManagementSystem.APIs.Errors;
    using OrderManagementSystem.Core;
    using OrderManagementSystem.Core.Entities;
    using OrderManagementSystem.Core.Entities.OrderAggregate;
    using OrderManagementSystem.Core.Entities.UserAggregate;
    using OrderManagementSystem.Core.Services.Contract;
    using System.Data;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;

    namespace OrderManagementSystem.APIs.Controllers
    {
    
        public class ProductController : BaseApiController
        {
            private readonly IProductService _productService;
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public ProductController(IProductService productService , IUserService userService , IMapper mapper)
            {

                _productService = productService;
                _userService = userService;
                _mapper = mapper;
            }

            [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
            [HttpGet]
            public async Task<ActionResult<ProductToReturnDto>> GetProducts()
            {
                var products = await _productService.GetAllProductsAsync();

                return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
            }

            [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
            [HttpGet("{id}")]
            public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProductById(int id)
            {

                var product = await _productService.GetProductByIdAsync(id);

                if (product is null) return NotFound(new ApiResponse(404)); //status 404

                return Ok(_mapper.Map<Product, ProductToReturnDto>(product)); //status 200
            }

            [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
            [HttpPost("insert")]
            [Authorize(Roles = "Admin")]
            public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductDto productDto)
            {
                if (productDto == null)
                {
                    return BadRequest(new ApiResponse(400));
                }

                var product = new Product
                {
                    ProductName = productDto.ProductName,
                    Price = productDto.Price,
                    Stock = productDto.Stock,
                };

                var prod = await _productService.CreateNewProductAsync(product);

                if (prod is null) return BadRequest(new ApiResponse(404));

                return Ok(prod);
            }


            [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
            [HttpPut("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] ProductDto productDto)
            {
                if (productDto == null)
                {
                    return BadRequest(new ApiResponse(400));
                }
                var product = new Product
                {
                    ProductName = productDto.ProductName,
                    Price = productDto.Price,
                    Stock = productDto.Stock,
                };

                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new ApiResponse(404));
                }

                // Update properties as needed
                existingProduct.ProductName = productDto.ProductName;
                existingProduct.Stock = productDto.Stock;
                existingProduct.Price = productDto.Price;
                // Update other properties...

                await _productService.UpdateProductAsync(existingProduct);
                return Ok("Order Data Updated Successfully"); // 204 No Content
            }
        }
    }

