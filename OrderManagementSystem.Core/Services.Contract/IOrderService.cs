using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<(Order Order, string Message)> CreateOrderAsync(Order order);
        Task<IReadOnlyList<Order>> GetAllOrdersAsync();

        Task<IReadOnlyList<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);

        Task<Order> GetOrderByIdAsync(int id);
        Task<IReadOnlyList<Order>> GetOrderByIdUsingSpecAsync(int id);
    }
}
