using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagementSystem.APIs.Controllers;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Tests
{
    public class CustomerControllerTests
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomerController _controller;
        public CustomerControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _mockOrderService = new Mock<IOrderService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CustomerController(_mockCustomerService.Object, _mockOrderService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsOkResult_WithCreatedCustomer()
        {
            // Arrange
            var customerDto = new CustomerDto
            {
                CustomerName = "John Doe",
                Email = "john.doe@example.com"
            };

            var customer = new Customer
            {
                CustomerName = customerDto.CustomerName,
                Email = customerDto.Email
            };

            _mockCustomerService.Setup(service => service.CreateNewCustomerAsync(It.IsAny<Customer>()))
                                .ReturnsAsync((customer, "Customer created successfully"));

            // Act
            var result = await _controller.CreateCustomer(customerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnCustomer = Assert.IsType<Customer>(okResult.Value);
            Assert.Equal(customer.CustomerName, returnCustomer.CustomerName);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsBadRequest_WhenCustomerDtoIsNull()
        {
            // Act
            var result = await _controller.CreateCustomer(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(400, apiResponse.StatusCode);
        }

        [Fact]
        public async Task CreateCustomer_ReturnsBadRequest_WhenCustomerCreationFails()
        {
            // Arrange
            var customerDto = new CustomerDto
            {
                CustomerName = "John Doe",
                Email = "john.doe@example.com"
            };

            _mockCustomerService.Setup(service => service.CreateNewCustomerAsync(It.IsAny<Customer>()))
                                .ReturnsAsync((null, "Error creating customer"));

            // Act
            var result = await _controller.CreateCustomer(customerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetOrdersForUser_ReturnsOkResult_WithListOfOrders()
        {
            // Arrange
            int userId = 1;
            var orders = new List<Order>
            {
                new Order { OrderId = 1, CustomerId = userId, OrderItems = new List<OrderItem>(), Invoice = new Invoice() }
            };

            var orderDtos = new List<OrderToReturnDto>
            {
                new OrderToReturnDto { OrderId = 1, CustomerId = userId }
            };

            _mockOrderService.Setup(service => service.GetOrdersByUserIdAsync(userId)).ReturnsAsync(orders);
            _mockMapper.Setup(m => m.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(It.IsAny<IReadOnlyList<Order>>()))
                       .Returns(orderDtos);

            // Act
            var result = await _controller.GetOrdersForUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnOrders = Assert.IsType<List<OrderToReturnDto>>(okResult.Value);
            Assert.Single(returnOrders);
        }

        [Fact]
        public async Task GetOrdersForUser_ReturnsBadRequest_WhenNoOrders()
        {
            // Arrange
            int userId = 1;
            _mockOrderService.Setup(service => service.GetOrdersByUserIdAsync(userId)).ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.GetOrdersForUser(userId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetOrdersForUser_ReturnsBadRequest_WhenInvalidUserId()
        {
            // Act
            var result = await _controller.GetOrdersForUser(-1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }

    }
}
