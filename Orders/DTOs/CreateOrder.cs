
namespace ConsoleOrderExecutor.Orders.DTOs
{
    public class CreateOrder
    {
        public required bool IsCompany { get; set; }
        public string? DeliveryAddress { get; set; }
        public required int StatusId { get; set; }
        public required int PaymentOptionId { get; set; }
    }
}
