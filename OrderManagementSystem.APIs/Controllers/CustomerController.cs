using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Application.ProductService;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;

namespace OrderManagementSystem.APIs.Controllers
{
    
    public class CustomerController : BaseApiController
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService , IOrderService orderService , IMapper mapper)
        {
            _customerService = customerService;
            _orderService = orderService;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("insert")]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest(new ApiResponse(400));
            }

            var customer = new Customer
            {
                CustomerName = customerDto.CustomerName,
                Email = customerDto.Email,
            };

            var(createdCustomer, message) = await _customerService.CreateNewCustomerAsync(customer);

            if (createdCustomer == null ) return BadRequest(new ApiResponse(404 , message));

            return Ok(createdCustomer);
        }

        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{userId}/orders")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(int userId)
        {
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            if (orders == null || orders.Count == 0)
            {
                return BadRequest(new ApiResponse(404));
            }

            return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
        }
    }
}
