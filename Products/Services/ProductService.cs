using ConsoleOrderExecutor.context;
using ConsoleOrderExecutor.Products.DTOs;

namespace ConsoleOrderExecutor.Products.Services
{
    public interface IPublicService
    {
        public bool CreateProduct(CreateProduct createProduct);
        public bool ProductExist(string ean);
        public bool ProductExist(int id);
        public bool ModifyProduct(ModifyProduct modifyProduct);
        public IEnumerable<GetProduct> GetProducts();
    }
    public class ProductService(ConsoleOrderExecutorDbContext context) : IPublicService
    {
        private readonly ConsoleOrderExecutorDbContext _context = context;

        public bool CreateProduct(CreateProduct createProduct)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GetProduct> GetProducts()
        {
            throw new NotImplementedException();
        }

        public bool ModifyProduct(ModifyProduct modifyProduct)
        {
            throw new NotImplementedException();
        }

        public bool ProductExist(string ean)
        {
            throw new NotImplementedException();
        }

        public bool ProductExist(int id)
        {
            throw new NotImplementedException();
        }
    }
}
