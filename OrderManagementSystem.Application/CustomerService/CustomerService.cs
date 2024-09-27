using OrderManagementSystem.Core;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(Customer Customer, string Message)> CreateNewCustomerAsync(Customer customer)
        {
            if (await _unitOfWork.Repository<Customer>().AnyAsync(c => c.Email == customer.Email))
            {
                return (null, "Email already exist");
            }

            _unitOfWork.Repository<Customer>().Add(customer);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return (null, "");

            return (customer, "Customer created successfully."); 
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _unitOfWork.Repository<Customer>().AnyAsync(c => c.CustomerId == customerId);
        }

        public async Task<string> GetCustomerEmailAsync(int customerId)
        {
            var customer =  await _unitOfWork.Repository<Customer>().GetByIdAsync(customerId);
            return customer.Email;
        }
    }
}
