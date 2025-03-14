using ConsoleOrderExecutor.context;
using ConsoleOrderExecutor.Orders.DTOs;

namespace ConsoleOrderExecutor.Orders.Services
{
    public interface IOrderService
    {
        public bool CreateOrder(CreateOrder createOrder);
        public bool ModifyOrder(ModifyOrder modifyOrder);
        public IEnumerable<GetOrder> GetOrders();
    }
    public class OrderService(ConsoleOrderExecutorDbContext context) : IOrderService
    {
        private readonly ConsoleOrderExecutorDbContext _context = context;

        public bool CreateOrder(CreateOrder createOrder)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GetOrder> GetOrders()
        {
            throw new NotImplementedException();
        }

        public bool ModifyOrder(ModifyOrder modifyOrder)
        {
            throw new NotImplementedException();
        }
    }
}
