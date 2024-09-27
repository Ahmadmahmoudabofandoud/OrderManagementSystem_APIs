using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using OrderManagementSystem.APIs.Controllers;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.EmailService;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Tests
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockCustomerService = new Mock<ICustomerService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new OrderController(_mockOrderService.Object, _mockEmailService.Object, _mockConfiguration.Object, _mockCustomerService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateOrder_ReturnsOkResult_WithCreatedOrder()
        {
            // Arrange
            var orderDto = new OrderDto
            {
                CustomerId = 1,
                OrderItems = new List<OrderItemDto>
                {
                    new OrderItemDto { ProductId = 1, Quantity = 1, UnitPrice = 100  }
                },
                PaymentMethod = "CreditCard"
            };

            var order = new Order
            {
                CustomerId = orderDto.CustomerId,
                OrderDate = DateTime.UtcNow,
                PaymentMethod = Enum.Parse<PaymentMethod>(orderDto.PaymentMethod),
                Status = OrderStatus.Pending,
                OrderItems = orderDto.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                }).ToList()
            };

            _mockOrderService.Setup(service => service.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync((order, "Order Created"));

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var createdOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(orderDto.CustomerId, createdOrder.CustomerId);
        }

        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenOrderDtoIsNull()
        {
            // Arrange
            OrderDto orderDto = null;

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(400, apiResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateOrderStatus_ReturnsOkResult_WithUpdatedStatus()
        {
            // Arrange
            int orderId = 1;
            var orderStatusDto = new OrderStatusDto { Status = "PaymentReceived" };

            _mockOrderService.Setup(service => service.UpdateOrderStatusAsync(orderId, OrderStatus.PaymentReceived)).ReturnsAsync(true);
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync(new Order { OrderId = orderId, CustomerId = 1, Status = OrderStatus.PaymentReceived });
            _mockCustomerService.Setup(service => service.GetCustomerEmailAsync(It.IsAny<int>())).ReturnsAsync("customer@example.com");
            _mockEmailService.Setup(service => service.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateOrderStatus(orderId, orderStatusDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnStatus = Assert.IsType<OrderStatusDto>(okResult.Value);
            Assert.Equal(orderStatusDto.Status, returnStatus.Status);
        }

        [Fact]
        public async Task GetAllOrders_ReturnsOkResult_WithListOfOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderId = 1 }
            };

            var orderDtos = new List<OrderToReturnDto>
            {
                new OrderToReturnDto { OrderId = 1 }
            };

            _mockOrderService.Setup(service => service.GetAllOrdersAsync()).ReturnsAsync(orders);
            _mockMapper.Setup(m => m.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(It.IsAny<IReadOnlyList<Order>>()))
                       .Returns(orderDtos);

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnOrders = Assert.IsType<List<OrderToReturnDto>>(okResult.Value);
            Assert.Single(returnOrders);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOkResult_WithOrder()
        {
            // Arrange
            int orderId = 1;
            var order = new Order { OrderId = orderId };
            var orderDto = new OrderToReturnDto { OrderId = orderId };

            _mockOrderService.Setup(service => service.GetOrderByIdUsingSpecAsync(orderId)).ReturnsAsync(new List<Order> { order });
            _mockMapper.Setup(m => m.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(It.IsAny<IReadOnlyList<Order>>()))
                       .Returns(new List<OrderToReturnDto> { orderDto });

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnOrder = Assert.IsType<List<OrderToReturnDto>>(okResult.Value);
            Assert.Single(returnOrder);
            Assert.Equal(orderDto.OrderId, returnOrder[0].OrderId);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            int orderId = 2;

            _mockOrderService.Setup(service => service.GetOrderByIdUsingSpecAsync(orderId)).ReturnsAsync(new List<Order>());

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }
    }
}
