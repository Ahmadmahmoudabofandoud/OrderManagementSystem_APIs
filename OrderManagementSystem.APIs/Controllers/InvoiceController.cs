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
    
    public class InvoiceController : BaseApiController
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IMapper _mapper;

        public InvoiceController(IInvoiceService invoiceService , IMapper mapper)
        {
            _invoiceService = invoiceService;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<InvoiceDto>>> GetInvoices()
        {

            var invoices = await _invoiceService.GetAllInvoicesAsync();

            return Ok(_mapper.Map<IReadOnlyList<Invoice>, IReadOnlyList<InvoiceDto>>(invoices));
        }

        [ProducesResponseType(typeof(Invoice), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceById(int id)
        {

            var invoice = await _invoiceService.GetInvoiceByIdUsingSpecAsync(id);

            if (invoice is null) return NotFound(new ApiResponse(404)); //status 404

            return Ok(_mapper.Map<IReadOnlyList<Invoice>, IReadOnlyList<InvoiceDto>>(invoice)); //status 200
        }

    }
}
