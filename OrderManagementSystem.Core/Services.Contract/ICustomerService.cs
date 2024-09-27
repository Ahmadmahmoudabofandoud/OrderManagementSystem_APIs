using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Services.Contract
{
    public interface ICustomerService
    {
        Task<(Customer Customer, string Message)> CreateNewCustomerAsync(Customer customer);
        Task<bool> CustomerExistsAsync(int customerId);

        Task<string> GetCustomerEmailAsync(int customerId);

    }
}
