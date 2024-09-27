using OrderManagementSystem.Core;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Services.Contract;
using OrderManagementSystem.Core.Specifications.InvoiceSpecifications;
using OrderManagementSystem.Core.Specifications.OrderSpecifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Application.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IReadOnlyList<Invoice>> GetAllInvoicesAsync()
        {
            var invoiceRepo = _unitOfWork.Repository<Invoice>();
            var spec = new InvoiceSpecifications();
            var invoices = await invoiceRepo.GetAllWithSpecAsync(spec);
            return invoices;
        }

        public async Task<IReadOnlyList<Invoice>> GetInvoiceByIdUsingSpecAsync(int id)
        {
            var invoiceRepo = _unitOfWork.Repository<Invoice>();
            var spec = new InvoiceSpecifications(id);
            var invoices = await invoiceRepo.GetAllWithSpecAsync(spec);
            return invoices;
        }
    }
}
