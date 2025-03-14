using ConsoleOrderExecutor.Products.Models;

namespace ConsoleOrderExecutor.Orders.Models;

public partial class OrderProduct
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public virtual AppOrder Order { get; set; } = null!;

    public virtual AppProduct Product { get; set; } = null!;
}
