using OrderManagementSystem.Core.Entities.OrderAggregate;

namespace OrderManagementSystem.APIs.DTOs
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
