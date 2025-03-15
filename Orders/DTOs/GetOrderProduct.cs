
namespace ConsoleOrderExecutor.Orders.DTOs
{
    public class GetOrderProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ean { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
