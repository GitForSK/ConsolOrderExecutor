
namespace ConsoleOrderExecutor.ConsoleFunction
{
    public interface IConsoleFunctions
    {
        public void CreateNewOrder();
        public void PassOrderToWarehouse();
        public void SendOrder();
        public void ShowOrders();
        public void ShowProducts();
        public void ModifyProduct();
    }
    public class ConsoleFunctions : IConsoleFunctions
    {
        public void CreateNewOrder()
        {
            throw new NotImplementedException();
        }

        public void ModifyProduct()
        {
            throw new NotImplementedException();
        }

        public void PassOrderToWarehouse()
        {
            throw new NotImplementedException();
        }

        public void SendOrder()
        {
            throw new NotImplementedException();
        }

        public void ShowOrders()
        {
            throw new NotImplementedException();
        }

        public void ShowProducts()
        {
            throw new NotImplementedException();
        }
    }
}
