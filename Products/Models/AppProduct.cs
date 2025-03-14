using ConsoleOrderExecutor.Orders.Models;

namespace ConsoleOrderExecutor.Products.Models;

public partial class AppProduct
{
    public int ProductId { get; set; }

    public string ProductEan { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
}
