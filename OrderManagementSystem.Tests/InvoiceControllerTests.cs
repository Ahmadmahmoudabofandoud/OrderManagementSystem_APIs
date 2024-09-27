using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagementSystem.APIs.Controllers;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core.Entities;
using OrderManagementSystem.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementSystem.Tests
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IInvoiceService> _mockInvoiceService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly InvoiceController _controller;
        public InvoiceControllerTests()
        {
            _mockInvoiceService = new Mock<IInvoiceService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new InvoiceController(_mockInvoiceService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetInvoices_ReturnsOkResult_WithListOfInvoices()
        {
            // Arrange
            var invoices = new List<Invoice>
            {
                new Invoice { InvoiceId = 1 }
            };

            var invoiceDtos = new List<InvoiceDto>
            {
                new InvoiceDto { InvoiceId = 1 }
            };

            _mockInvoiceService.Setup(service => service.GetAllInvoicesAsync()).ReturnsAsync(invoices);
            _mockMapper.Setup(m => m.Map<IReadOnlyList<Invoice>, IReadOnlyList<InvoiceDto>>(It.IsAny<IReadOnlyList<Invoice>>()))
                       .Returns(invoiceDtos);

            // Act
            var result = await _controller.GetInvoices();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnInvoices = Assert.IsType<List<InvoiceDto>>(okResult.Value);
            Assert.Single(returnInvoices);
        }

        [Fact]
        public async Task GetInvoiceById_ReturnsOkResult_WithInvoice()
        {
            // Arrange
            int invoiceId = 1;
            var invoice = new Invoice { InvoiceId = invoiceId };
            var invoiceDto = new InvoiceDto { InvoiceId = invoiceId };

            _mockInvoiceService.Setup(service => service.GetInvoiceByIdUsingSpecAsync(invoiceId)).ReturnsAsync(new List<Invoice> { invoice });
            _mockMapper.Setup(m => m.Map<IReadOnlyList<Invoice>, IReadOnlyList<InvoiceDto>>(It.IsAny<IReadOnlyList<Invoice>>()))
                       .Returns(new List<InvoiceDto> { invoiceDto });

            // Act
            var result = await _controller.GetInvoiceById(invoiceId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnInvoice = Assert.IsType<List<InvoiceDto>>(okResult.Value);
            Assert.Single(returnInvoice);
            Assert.Equal(invoiceDto.InvoiceId, returnInvoice[0].InvoiceId);
        }

        [Fact]
        public async Task GetInvoiceById_ReturnsNotFound_WhenInvoiceDoesNotExist()
        {
            // Arrange
            int invoiceId = 1;

            _mockInvoiceService.Setup(service => service.GetInvoiceByIdUsingSpecAsync(invoiceId)).ReturnsAsync((List<Invoice>)null);

            // Act
            var result = await _controller.GetInvoiceById(invoiceId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
        }
    }
}
