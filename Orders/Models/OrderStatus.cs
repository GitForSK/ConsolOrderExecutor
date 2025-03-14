namespace ConsoleOrderExecutor.Orders.Models;

public partial class OrderStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<AppOrder> AppOrders { get; set; } = new List<AppOrder>();
}
