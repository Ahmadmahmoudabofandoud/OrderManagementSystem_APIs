using OrderManagementSystem.Core;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;
using OrderManagementSystem.Core.Specifications.OrderSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;

        public OrderService(IProductService productService 
            , IUnitOfWork unitOfWork
            
            , ICustomerService customerService)
        {
            _productService = productService;
            _unitOfWork = unitOfWork;
            _customerService = customerService;
        }
        public async Task<(Order Order, string Message)> CreateOrderAsync(Order order)
        {

            //check if customer exist or not
            var customerExists = await _customerService.CustomerExistsAsync(order.CustomerId);
            if (!customerExists)
            {
                return (null, "Customer does not exist.");
            }

            decimal totalAmount = 0;

            
            //Validations
            foreach (var item in order.OrderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                //check product stock
                if (product.Stock < item.Quantity) // Check stock
                {
                    return (null, $"Product:{product.ProductName} is out of stock.");
                }

                //check product Exist or not
                if (product == null)
                {
                    return (null, $"Product does not exist.");
                }

                //check product price
                if (item.UnitPrice != product.Price)
                {
                    return (null, $"Product: {product.ProductName} price is incorrect.");
                }
                totalAmount += item.Quantity * item.UnitPrice;
            }


            ApplyDiscounts(order, totalAmount);

            //Create Invoice
            var invoice = new Invoice
            {
                OrderDate = DateTime.Now,
                TotalAmount = CalculateTotalAmount(order),
                Order = order
            };



            
            //Create Order and invoice
            _unitOfWork.Repository<Order>().Add(order);
            _unitOfWork.Repository<Invoice>().Add(invoice);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return (null, "");


            // Update stock for each order item
            foreach (var item in order.OrderItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                product.Stock -= item.Quantity; // Reduce the stock
                await _productService.UpdateProductAsync(product); // Update the product in the repository
            }

            return (order, "Order created successfully."); ;

        }

        public async Task<IReadOnlyList<Order>> GetAllOrdersAsync()
        {
            var ordersRepo =  _unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications();
            var orders = await ordersRepo.GetAllWithSpecAsync(spec);
            return orders;
        }


        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(id);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrderByIdUsingSpecAsync(int id)
        {
            var ordersRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications(id);
            var orders = await ordersRepo.GetAllWithSpecAsync(spec);
            return orders;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var ordersRepo = _unitOfWork.Repository<Order>();
            var spec = new OrderByUserSpecifications(userId);
            var orders = await ordersRepo.GetAllWithSpecAsync(spec);
            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order == null)
            {
                return false;
            }
            order.Status = newStatus;

            _unitOfWork.Repository<Order>().Update(order);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;

        }

        private void ApplyDiscounts(Order order, decimal totalAmount)
        {
            decimal discount = 0;

            if (totalAmount > 200)
            {
                discount = 0.10m; // 10% discount for orders over $200
            }
            else if (totalAmount > 100)
            {
                discount = 0.05m; // 5% discount for orders over $100
            }

            foreach (var item in order.OrderItems)
            {
                item.Discount = discount; // Set the discount for each order item
                item.UnitPrice -= item.UnitPrice * discount;
            }
        }

        private decimal CalculateTotalAmount(Order order)
        {
            decimal total = 0;
            foreach (var item in order.OrderItems)
            {
                total += item.Quantity * item.UnitPrice * (1 - item.Discount); // Apply discount
            }
            return total;
        }

        
    }
}
