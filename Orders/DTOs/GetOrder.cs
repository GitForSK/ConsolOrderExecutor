
using ConsoleOrderExecutor.Products.DTOs;

namespace ConsoleOrderExecutor.Orders.DTOs
{
    public class GetOrder
    {
        public int Id { get; set; }
        public decimal OrderValue { get; set; }
        public IEnumerable<GetProduct> Products { get; set; } = Enumerable.Empty<GetProduct>();
        public string OrderType { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = "Null";
        public string StatusName { get; set; } = string.Empty;
        public string PaymentOption { get; set; } = string.Empty;
    }
}
