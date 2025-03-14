
namespace ConsoleOrderExecutor.Orders.DTOs
{
    public class ModifyOrder
    {
        public required int Id { get; set; }
        public bool? IsCompany { get; set; }
        public string? DeliveryAddress { get; set; }
        public int? StatusId { get; set; }
        public int? PaymentOptionId { get; set; }
    }
}
