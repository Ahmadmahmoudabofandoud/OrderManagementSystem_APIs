using OrderManagementSystem.Core.Entities.OrderAggregate;
using OrderManagementSystem.Core.Entities;

namespace OrderManagementSystem.APIs.DTOs
{
    public class OrderToReturnDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public int InvoiceId { get; set; }
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; } = new HashSet<OrderItemDto>();
    }
}
