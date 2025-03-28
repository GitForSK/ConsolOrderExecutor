﻿using ConsoleOrderExecutor.context;
using ConsoleOrderExecutor.Orders.DTOs;
using ConsoleOrderExecutor.Orders.Models;
using Microsoft.EntityFrameworkCore;

namespace ConsoleOrderExecutor.Orders.Services
{
    public interface IOrderService
    {
        public Task<bool> CreateOrder(CreateOrder createOrder);
        public Task<bool> ModifyOrder(ModifyOrder modifyOrder);
        public Task<IEnumerable<GetOrder>> GetOrders();
        public Task<int> GetStatusId(string name);
        public Task<IEnumerable<GetPaymentOption>> GetPaymentOptions();
        public Task<bool> OrderExist(int orderId);
        public Task<int> GetOrderStatusId(int orderId);
        public Task<decimal> GetOrderValue(int orderId);
        public Task<int> GetPaymentOptionId(string name);
        public Task<int> GetOrderPaymentOptionId(int orderId);
    }
    public class OrderService(ConsoleOrderExecutorDbContext context) : IOrderService
    {
        private readonly ConsoleOrderExecutorDbContext _context = context;
        /// <summary>
        /// Using transactions create new order and then add existing items to it.
        /// </summary>
        /// <param name="createOrder">Parameters of new order.</param>
        /// <returns>True if order was successfully created, otherwise false.</returns>
        public async Task<bool> CreateOrder(CreateOrder createOrder)
        {
            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                var newOrder = new AppOrder
                {
                    IsCompany = createOrder.IsCompany,
                    DeliveryAddress = createOrder.DeliveryAddress,
                    StatusId = createOrder.StatusId,
                    PaymentOptionId = createOrder.PaymentOptionId,
                };
                await _context.AppOrders.AddAsync(newOrder);
                await _context.SaveChangesAsync();

                var orderItems = createOrder.Products.Select(x => new OrderProduct
                {
                    OrderId = newOrder.OrderId,
                    ProductId = x.Id,
                    Price = x.Price,
                }).ToList();

                await _context.OrderProducts.AddRangeAsync(orderItems);
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
        /// Query for id of status with given name.
        /// </summary>
        /// <param name="name">Status name (case sensitive).</param>
        /// <returns>Id of status or default int.</returns>
        public async Task<int> GetStatusId(string name)
        {
            return await _context.OrderStatuses.Where(x => x.StatusName == name).Select(x => x.StatusId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Query database to receive information about orders.
        /// </summary>
        /// <returns>List of GetOrder objects.</returns>
        public async Task<IEnumerable<GetOrder>> GetOrders()
        {
            return await _context.AppOrders.Select(x => new GetOrder
            {
                Id = x.OrderId,
                OrderValue = x.OrderProducts.Sum(a => a.Price),
                Products = x.OrderProducts.Select(a => new GetOrderProduct
                {
                    Id = a.ProductId,
                    Name = a.Product.ProductName,
                    Ean = a.Product.ProductEan,
                    Price = a.Price,
                }),
                OrderType = x.IsCompany ? "Company" : "Physical person",
                DeliveryAddress = x.DeliveryAddress ?? "Null",
                StatusName = x.Status.StatusName,
                PaymentOption = x.PaymentOption.OptionName,
            }).ToListAsync();
        }
        /// <summary>
        /// Query database for status information.
        /// </summary>
        /// <returns>List of GetPaymentOption objects.</returns>
        public async Task<IEnumerable<GetPaymentOption>> GetPaymentOptions()
        {
            return await _context.PaymentOptions.Select(x => new GetPaymentOption
            {
                Id = x.PaymentOptionId,
                Name = x.OptionName
            }).ToListAsync();
        }
        /// <summary>
        /// Using transactions modify order.
        /// </summary>
        /// <param name="modifyOrder">New order parameters. If parameter in given object is null then the value will not be changed.</param>
        /// <returns>True if successfully created, otherwise false.</returns>
        public async Task<bool> ModifyOrder(ModifyOrder modifyOrder)
        {
            using var trans = await _context.Database.BeginTransactionAsync();
            try
            {
                if (modifyOrder.IsCompany != null)
                {
                    await _context.AppOrders
                        .Where(x => x.OrderId == modifyOrder.Id)
                    .ExecuteUpdateAsync(setter => setter
                            .SetProperty(s => s.IsCompany, modifyOrder.IsCompany));
                }

                if (modifyOrder.DeliveryAddress != null)
                {
                    await _context.AppOrders
                        .Where(x => x.OrderId == modifyOrder.Id)
                    .ExecuteUpdateAsync(setter => setter
                            .SetProperty(s => s.DeliveryAddress, modifyOrder.DeliveryAddress));
                }

                if (modifyOrder.StatusId != null)
                {
                    await _context.AppOrders
                        .Where(x => x.OrderId == modifyOrder.Id)
                    .ExecuteUpdateAsync(setter => setter
                            .SetProperty(s => s.StatusId, modifyOrder.StatusId));
                }

                if (modifyOrder.PaymentOptionId != null)
                {
                    await _context.AppOrders
                        .Where(x => x.OrderId == modifyOrder.Id)
                    .ExecuteUpdateAsync(setter => setter
                            .SetProperty(s => s.PaymentOptionId, modifyOrder.PaymentOptionId));
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
        /// Check if order with given id exists.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>True if exist, false otherwise.</returns>
        public async Task<bool> OrderExist(int orderId)
        {
            return await _context.AppOrders.AnyAsync(x => x.OrderId == orderId);
        }
        /// <summary>
        /// Query for status id of given order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Status id of given order or 0 if not found.</returns>
        public async Task<int> GetOrderStatusId(int orderId)
        {
            return await _context.AppOrders.Where(x => x.OrderId == orderId).Select(x => x.StatusId).FirstAsync();
        }
        /// <summary>
        /// Query for value of order with given id.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Value of the order.</returns>
        public async Task<decimal> GetOrderValue(int orderId)
        {
            return await _context.OrderProducts.Where(x => x.OrderId == orderId).SumAsync(x => x.Price);
        }
        /// <summary>
        /// Query database for payment option id with given name.
        /// </summary>
        /// <param name="name">Name of payment option (case sensitive).</param>
        /// <returns>Payment option id or 0 if not found.</returns>
        public async Task<int> GetPaymentOptionId(string name)
        {
            return await _context.PaymentOptions.Where(x => x.OptionName == name).Select(x => x.PaymentOptionId).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Query database for payment option id of given order.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <returns>Payment option id or default int if not found.</returns>
        public async Task<int> GetOrderPaymentOptionId(int orderId)
        {
            return await _context.AppOrders.Where(x => x.OrderId == orderId).Select(x => x.PaymentOptionId).FirstOrDefaultAsync();
        }
    }
}
