namespace ConsoleOrderExecutor.Orders.Models;

public partial class AppOrder
{
    public int OrderId { get; set; }

    public bool IsCompany { get; set; }

    public string? DeliveryAddress { get; set; }

    public int StatusId { get; set; }

    public int PaymentOptionId { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual PaymentOption PaymentOption { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;
}
