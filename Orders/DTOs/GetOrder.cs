namespace ConsoleOrderExecutor.Orders.DTOs
{
    public class GetOrder
    {
        public int Id { get; set; }
        public decimal OrderValue { get; set; }
        public IEnumerable<GetOrderProduct> Products { get; set; } = Enumerable.Empty<GetOrderProduct>();
        public string OrderType { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string PaymentOption { get; set; } = string.Empty;
    }
}
