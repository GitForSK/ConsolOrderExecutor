namespace ConsoleOrderExecutor.Orders.Models;

public partial class PaymentOption
{
    public int PaymentOptionId { get; set; }

    public string OptionName { get; set; } = null!;

    public virtual ICollection<AppOrder> AppOrders { get; set; } = new List<AppOrder>();
}
