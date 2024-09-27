namespace OrderManagementSystem.APIs.DTOs
{
    public class ProductToReturnDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
