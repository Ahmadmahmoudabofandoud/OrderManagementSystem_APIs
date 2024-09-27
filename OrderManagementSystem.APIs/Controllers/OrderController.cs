using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.EmailService;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Application.ProductService;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;
using System.Text;

namespace OrderManagementSystem.APIs.Controllers
{
    
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _conf;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService 
            , IEmailService emailService 
            , IConfiguration conf
            , ICustomerService customerService
            , IMapper mapper)
        {
            _orderService = orderService;
            _emailService = emailService;
            _conf = conf;
            _customerService = customerService;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("insert")]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || !orderDto.OrderItems.Any())
            {
                return BadRequest(new ApiResponse(400));
            }

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

            var (createdOrder, message) = await _orderService.CreateOrderAsync(order);


            if (createdOrder == null)
            {
                return BadRequest(new ApiResponse(400 , message)); // Return error message with 400 status
            }
            return Ok(createdOrder);
        }

        [ProducesResponseType(typeof(OrderStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize(Roles ="Admin")]
        [HttpPut("{orderId}/status")]
        public async Task<ActionResult<OrderStatusDto>> UpdateOrderStatus(int orderId, [FromBody] OrderStatusDto orderStatusDto)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), orderStatusDto.Status))
            {
                return BadRequest("Invalid order status.");
            }

            var statusEnum = Enum.Parse<OrderStatus>(orderStatusDto.Status);

            var updateResult = await _orderService.UpdateOrderStatusAsync(orderId, statusEnum);
            if (!updateResult)
            {
                return NotFound("Order not found or failed to update order status.");
            }

            var order = await _orderService.GetOrderByIdAsync(orderId);

            var customerEmail = await _customerService.GetCustomerEmailAsync(order.CustomerId);

            if (string.IsNullOrEmpty(customerEmail))
            {
                return BadRequest(new ApiResponse(500, "Customer email not found or empty."));
            }


            var emailSubject = "Order Status Updated";
            var emailBody = $"Dear Customer,\n\nYour order (ID: {order.OrderId}) status has been updated to {order.Status}.\n\nThank you for shopping with us.";
            // Send the email
            var emailResult = await _emailService.SendEmailAsync(_conf["EmailSettings:senderEmail"], customerEmail, emailSubject, emailBody);
            if (!emailResult)
            {
                return StatusCode(500, "Order status updated but failed to send email notification.");
            }



            return Ok(orderStatusDto);
        }

        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders()
        {
            
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrderById(int id)
        {
            var orders = await _orderService.GetOrderByIdUsingSpecAsync(id);
            if (orders == null || orders.Count == 0)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }
            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }

    }
}
