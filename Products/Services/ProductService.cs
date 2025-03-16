using ConsoleOrderExecutor.context;
using ConsoleOrderExecutor.Products.DTOs;
using ConsoleOrderExecutor.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleOrderExecutor.Products.Services
{
    public interface IProductService
    {
        public Task<bool> CreateProduct(CreateProduct createProduct);
        public Task<bool> ProductExist(string ean);
        public Task<bool> ProductExist(int id);
        public Task<int> GetProductId(string ean);
        public Task<bool> ModifyProduct(ModifyProduct modifyProduct);
        public Task<IEnumerable<GetProduct>> GetProducts();
        public Task<GetProductInfo?> GetProductInfo(int id);
    }
    public class ProductService(ConsoleOrderExecutorDbContext context) : IProductService
    {
        private readonly ConsoleOrderExecutorDbContext _context = context;
        /// <summary>
        /// Using transaction create new products.
        /// </summary>
        /// <param name="createProduct">Products parameters</param>
        /// <returns>True if created, false if not.</returns>
        public async Task<bool> CreateProduct(CreateProduct createProduct)
        {
            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                var newProduct = new AppProduct
                {
                    ProductEan = createProduct.Ean,
                    ProductName = createProduct.Name,
                };

                await _context.AppProducts.AddAsync(newProduct);
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Query for id of product with given ean.
        /// </summary>
        /// <param name="ean">Product ean.</param>
        /// <returns>Id of product or default int.</returns>
        public async Task<int> GetProductId(string ean)
        {
            return await _context.AppProducts.Where(x => x.ProductEan == ean).Select(x => x.ProductId).FirstOrDefaultAsync();
        }

        public async Task<GetProductInfo?> GetProductInfo(int id)
        {
            return await _context.AppProducts.Where(x => x.ProductId == id).Select(x => new GetProductInfo
            {
                Name = x.ProductName,
                Ean = x.ProductEan
            }).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Query the database for products information.
        /// </summary>
        /// <returns>Return list of GetProduct objects.</returns>
        public async Task<IEnumerable<GetProduct>> GetProducts()
        {
            return await _context.AppProducts.Select(x => new GetProduct
            {
                Id = x.ProductId,
                Name = x.ProductName,
                Ean = x.ProductEan
            }).ToListAsync();
        }
        /// <summary>
        /// Using transactions update product with given id.
        /// </summary>
        /// <param name="modifyProduct">New parameters of product. If parameter in given object is null then the value will not be changed.</param>
        /// <returns>True if product was successfully modified, otherwise false.</returns>
        public async Task<bool> ModifyProduct(ModifyProduct modifyProduct)
        {
            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                if (modifyProduct.Ean != null)
                {
                    await _context.AppProducts
                        .Where(x => x.ProductId == modifyProduct.Id)
                        .ExecuteUpdateAsync(setter => setter
                            .SetProperty(s => s.ProductEan, modifyProduct.Ean));
                }
                if (modifyProduct.Name != null)
                {
                    await _context.AppProducts
                        .Where(x => x.ProductId == modifyProduct.Id)
                        .ExecuteUpdateAsync(setter => setter
                            .SetProperty(s => s.ProductName, modifyProduct.Name));
                }
                await _context.SaveChangesAsync();
                await trans.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await trans.RollbackAsync();
                return false;
            }
        }
        /// <summary>
        /// Check if product exist.
        /// </summary>
        /// <param name="ean">Searched ean.</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> ProductExist(string ean)
        {
            return await _context.AppProducts.AnyAsync(x => x.ProductEan == ean);
        }
        /// <summary>
        /// Check if product exist.
        /// </summary>
        /// <param name="id">Searched id.</param>
        /// <returns>True if exist, false if not.</returns>
        public async Task<bool> ProductExist(int id)
        {
            return await _context.AppProducts.AnyAsync(x => x.ProductId == id);
        }
    }
}
