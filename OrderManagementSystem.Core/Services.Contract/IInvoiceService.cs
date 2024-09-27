using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Core.Services.Contract
{
    public interface IInvoiceService
    {
        Task<IReadOnlyList<Invoice>> GetAllInvoicesAsync();
        Task<IReadOnlyList<Invoice>> GetInvoiceByIdUsingSpecAsync(int id);

    }
}
