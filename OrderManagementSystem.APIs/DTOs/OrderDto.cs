using OrderManagementSystem.Core.Entities.OrderAggregate;

namespace OrderManagementSystem.APIs.DTOs
{
    public class OrderDto
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
