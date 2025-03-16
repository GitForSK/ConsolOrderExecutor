
using ConsoleOrderExecutor.ConsoleFunction.Utils;
using ConsoleOrderExecutor.Orders.DTOs;
using ConsoleOrderExecutor.Orders.Services;
using ConsoleOrderExecutor.Products.DTOs;
using ConsoleOrderExecutor.Products.Services;
using System.Text.RegularExpressions;

namespace ConsoleOrderExecutor.ConsoleFunction
{
    public interface IConsoleFunctions
    {
        public Task CreateNewOrder();
        public Task PassOrderToWarehouse();
        public Task SendOrder();
        public Task ShowOrders();
        public Task ShowProducts();
        public Task ModifyProduct();
    }
    public class ConsoleFunctions(IConsoleUtils consoleUtils, IOrderService orderService, IProductService productService) : IConsoleFunctions
    {
        private readonly IConsoleUtils _consoleUtils = consoleUtils;
        private readonly IOrderService _orderService = orderService;
        private readonly IProductService _productService = productService;
        /// <summary>
        /// Try to receive parameters for user to create new order. Then write the outcome of action in console.
        /// </summary>
        public async Task CreateNewOrder()
        {
            Console.WriteLine("Process of creating new order started. If you want to exit process write exit at any step.");
            bool wantToExit = false;

            string isCompanyText = "Is this order for company or physical person? Type 1 if company or 0 if person.";
            wantToExit = _consoleUtils.GetParameter(isCompanyText, (a) => a != null && (a == "1" || a == "0"), out var isCompanyStr);
            if (wantToExit) return;

            string addressText = "Please write delivery address (max 250 characters).";
            wantToExit = _consoleUtils.GetParameter(addressText, (a) => (a ?? "").Length <= 250, out var address);
            if (wantToExit) return;

            var paymentList = await _orderService.GetPaymentOptions();
            var paymentInfoInString = paymentList.Select(x => x.Id + " - " + x.Name);
            string paymentText = "Please choose the payment option, by writing its number.\n" + String.Join("\n", paymentInfoInString);
            wantToExit = _consoleUtils.GetParameter(paymentText, (a) => (a ?? "").All(Char.IsDigit) && paymentList.Any(x => x.Id == Int32.Parse(a ?? "-1")), out var paymentStr);
            if (wantToExit) return;

            Console.WriteLine("Now please add products.");
            bool isAddOn = true;
            var newProducts = new List<NewOrderProduct>();
            var eanText = "Please write ean of product you want to add.";
            var nameText = "Please write product name.";
            var priceText = "Please write product price.";
            var nextText = "If you done write done, otherwise next.";

            while (isAddOn)
            {
                wantToExit = _consoleUtils.GetParameter(eanText, (a) => a != null && (a ?? "").Length < 13, out var newEan);
                if (wantToExit) return;
                Console.WriteLine("Checking if ean exist...");
                var eanExist = await _productService.ProductExist(newEan);
                int prodId = 0;
                string? name = "";
                if (eanExist)
                {
                    Console.WriteLine("Ean exist, adding product...");
                    prodId = await _productService.GetProductId(newEan);
                }
                else
                {
                    wantToExit = _consoleUtils.GetParameter(nameText, (a) => a != null && (a ?? "").Length < 150, out name);
                    if (wantToExit) return;
                }

                wantToExit = _consoleUtils.GetParameter(priceText, (a) => Regex.IsMatch(a ?? "", "(\\d+)|(\\d+\\.\\d{2})"), out var priceStr);
                if (wantToExit) return;

                Console.WriteLine("Adding product...");

                decimal price = decimal.Parse(priceStr);

                if (!eanExist)
                {
                    if (name == null)
                    {
                        Console.WriteLine("Error: Product name is null.");
                        return;
                    }
                    bool isAdded = await _productService.CreateProduct(new CreateProduct
                    {
                        Name = name,
                        Ean = newEan,
                    });
                    if (!isAdded)
                    {
                        Console.WriteLine("Error: Could not add product.");
                        return;
                    }
                    else
                    {
                        prodId = await _productService.GetProductId(newEan);
                    }
                }

                newProducts.Add(new NewOrderProduct
                {
                    Id = prodId,
                    Price = price,
                });

                Console.WriteLine("Product added.");

                wantToExit = _consoleUtils.GetParameter(nextText, (a) => a == "done" || a == "next", out var nextStep);
                if (wantToExit) return;

                isAddOn = nextStep == "next";
            }
            Console.WriteLine("Creating order...");
            string newStatusName = address == null || address == "" ? "Błąd" : "Nowe";
            if (address == null)
            {
                Console.WriteLine("Warning: Address do not posses value. Order status will be moved to Błąd.");
            }
            int statusId = await _orderService.GetStatusId(newStatusName);
            var newOrder = new CreateOrder
            {
                IsCompany = isCompanyStr == "1",
                DeliveryAddress = address,
                StatusId = statusId,
                PaymentOptionId = Int32.Parse(paymentStr),
                Products = newProducts,
            };

            bool isOrderCreated = await _orderService.CreateOrder(newOrder);
            if (isOrderCreated)
            {
                Console.WriteLine("Order created.");
            }
            else
            {
                Console.WriteLine("Failed to create order.");
            }
        }
        /// <summary>
        /// Take user input and modify the given product.
        /// </summary>
        public async Task ModifyProduct()
        {
            bool wantToExit = false;

            Console.WriteLine("You can exit the process at any time, by writing exit.");

            string getProductIdText = "Please write the id of the product that you want to modify.";
            wantToExit = _consoleUtils.GetParameter(getProductIdText, (a) => Regex.IsMatch((a ?? ""), "\\d+"), out var productIdStr);
            if (wantToExit) return;

            int productId = Int32.Parse(productIdStr);
            bool productExist = await _productService.ProductExist(productId);
            if (!productExist)
            {
                Console.WriteLine($"Error: The product with id {productId} do not exists.");
                return;
            }

            var productOldInfo = await _productService.GetProductInfo(productId);
            if (productOldInfo == null)
            {
                Console.WriteLine("Error: Could not find product information form database.");
                return;
            }

            Console.WriteLine($"Old product name: {productOldInfo.Name}");

            string getNewNameText = "Pass new product name or nothing if you do not want to change it.";
            wantToExit = _consoleUtils.GetParameter(getNewNameText, (a) => (a ?? "").Length < 150, out var newName);
            if (wantToExit) return;

            Console.WriteLine($"Old product ean: {productOldInfo.Ean}");

            string getNewEanText = "Pass new product ean or nothing if you do not want to change it.";
            wantToExit = _consoleUtils.GetParameter(getNewEanText, (a) => (a ?? "").Length < 13, out var newEan);
            if (wantToExit) return;

            if (newEan != null)
            {
                bool eanAlreadyExist = await _productService.ProductExist(newEan);
                if (eanAlreadyExist)
                {
                    Console.WriteLine("Error: Product with this ean already exist.");
                    return;
                }
            }

            bool isModified = await _productService.ModifyProduct(new ModifyProduct { 
                Id = productId,
                Name = newName,
                Ean = newEan,
            });

            if (isModified)
            {
                Console.WriteLine($"Successfully changed the product with id {productId}.");
            } else
            {
                Console.WriteLine("Error: Could not change product.");
            }
        }
        /// <summary>
        /// Take input from user to change order status to W magazynie. If order value exceed 2500 and payment option equal Gotówka przy odbiorze the satus will be changed to Zwrócone do klienta.
        /// </summary>
        public async Task PassOrderToWarehouse()
        {
            bool wantToExit = false;

            Console.WriteLine("You can exit the process at any time, by writing exit.");

            string getOrderIdText = "Please write the id of the order that you want to move to warehouse.";
            wantToExit = _consoleUtils.GetParameter(getOrderIdText, (a) => Regex.IsMatch((a ?? ""), "\\d+"), out var orderIdStr);
            if (wantToExit) return;

            Console.WriteLine("Checking order..");
            int orderId = Int32.Parse(orderIdStr);
            bool orderExist = await _orderService.OrderExist(orderId);

            if (!orderExist)
            {
                Console.WriteLine($"Error: The order with id {orderId} do not exist in database.");
                return;
            }

            decimal orderValue = await _orderService.GetOrderValue(orderId);

            int paymentOptionIdForValidation = await _orderService.GetPaymentOptionId("Gotówka przy odbiorze");
            if (paymentOptionIdForValidation == 0)
            {
                Console.WriteLine("Error: Could not find payment status with the name Gotówka przy odbiorze");
                return;
            }

            string newOrderStatus = "W magazynie";
            int orderPaymentOptionId = await _orderService.GetOrderPaymentOptionId(orderId);
            if (orderValue > 2500 && (paymentOptionIdForValidation == orderPaymentOptionId)) {
                Console.WriteLine("Waring: The order have payment option set as Gotówka przy odbiorze and it value exceed 2500. The status will be changed to Zwrócono do klienta.");
                newOrderStatus = "Zwrócono do klienta";
            }

            int statusNewId = await _orderService.GetStatusId("Nowe");
            if (statusNewId == 0)
            {
                Console.WriteLine("Error: Could not find status with the name Nowe");
                return;
            }
            int currentStatusId = await _orderService.GetOrderStatusId(orderId);

            if (currentStatusId != statusNewId)
            {
                Console.WriteLine("Error: Cannot change given order status, because the order status is not Nowe.");
                return;
            }

            int newStatusId = await _orderService.GetStatusId(newOrderStatus);
            if (newStatusId == 0)
            {
                Console.WriteLine($"Error: Could not find status with the name {newOrderStatus}");
                return;
            }

            bool statusHasChanged = await _orderService.ModifyOrder(new ModifyOrder
            {
                Id = orderId,
                StatusId = newStatusId,
            });

            if (statusHasChanged)
            {
                Console.WriteLine($"Order status with id {orderId} has been changed to {newOrderStatus}.");
            } else
            {
                Console.WriteLine($"Error: Could not change order status with id {orderId}.");
            }
        }
        /// <summary>
        /// Attempt to change order status from W magazynie to W wysyłce.
        /// </summary>
        public async Task SendOrder()
        {
            bool wantToExit = false;

            Console.WriteLine("You can exit the process at any time, by writing exit.");

            string getOrderIdText = "Please write the id of the order that you want to send.";
            wantToExit = _consoleUtils.GetParameter(getOrderIdText, (a) => Regex.IsMatch((a ?? ""), "\\d+"), out var orderIdStr);
            if (wantToExit) return;

            Console.WriteLine("Checking order..");
            int orderId = Int32.Parse(orderIdStr);
            bool orderExist = await _orderService.OrderExist(orderId);

            if (!orderExist)
            {
                Console.WriteLine($"Error: The order with id {orderId} do not exist in database.");
                return;
            }

            int statusWarehouseId = await _orderService.GetStatusId("W magazynie");
            if (statusWarehouseId == 0)
            {
                Console.WriteLine("Error: Could not find status with the name W magazynie");
                return;
            }
            int currentStatusId = await _orderService.GetOrderStatusId(orderId);

            if (currentStatusId != statusWarehouseId) {
                Console.WriteLine("Error: Cannot change given order status, because the order status is not W magazynie.");
                return;
            }

            int statusSendId = await _orderService.GetStatusId("W wysyłce");
            if (statusSendId == 0)
            {
                Console.WriteLine("Error: Could not find status with the name W wysyłce");
                return;
            }

            Thread changeStatus = new Thread(new ThreadStart(async () =>
            {
                Thread.Sleep(5000);
                await _orderService.ModifyOrder(new ModifyOrder
                {
                    Id = orderId,
                    StatusId = statusSendId,
                });
            }));

            changeStatus.Start();

            Console.WriteLine($"The order has been sent for shipment. Please check later if status of order {orderId} has been changed.");

        }
        /// <summary>
        /// Retrieve order list from database and then show the first 5 of them. If user do not write exit, shown another five till the end of the list.
        /// </summary>
        public async Task ShowOrders()
        {
            int pagination = 5;
            static void showOrder(GetOrder order) {
                Console.WriteLine($"id: {order.Id} value: {order.OrderValue} PLN status: {order.StatusName} payment option: {order.PaymentOption}");
                Console.WriteLine($"type: {order.OrderType} address: {order.DeliveryAddress}");
                Console.WriteLine("Products:");
                var prod = order.Products.Select(x => $"id: {x.Id} ean: {x.Ean} name: {x.Name} price: {x.Price}");
                Console.WriteLine(String.Join("\n", prod));
                Console.WriteLine(Environment.NewLine);
            };
            Console.WriteLine("Loading orders..");

            var orders = await _orderService.GetOrders();
            var ordersCount = orders.Count();

            Console.WriteLine($"There are {ordersCount} orders.");

            if (ordersCount < pagination)
            {
                foreach (var order in orders)
                {
                    showOrder(order);
                }
            }
            else
            {
                var dicOrders = orders.Index().ToDictionary();
                int counter = 0;
                while (counter < ordersCount)
                {
                    dicOrders.TryGetValue(counter, out var order);
                    if (order == null)
                    {
                        Console.WriteLine("Error: Found null order.");
                    }
                    else
                    {
                        showOrder(order);
                    }
                    if (counter != 0 && counter % pagination == 0)
                    {
                        Console.WriteLine("If you want to exit write exit. If you want to see more click enter or write anything.");
                        var userInput = Console.ReadLine();
                        if (userInput == "exit")
                        {
                            return;
                        }
                    }
                    counter++;
                }
            }

            Console.WriteLine("End of orders.");
        }
        /// <summary>
        /// Retrieve product list from database and then show the first 5 of them. If user do not write exit, shown another five till the end of the list.
        /// </summary>
        public async Task ShowProducts()
        {
            int pagination = 5;
            Console.WriteLine("Loading products..");

            var products = await _productService.GetProducts();
            var prodCount = products.Count();

            Console.WriteLine($"There are {prodCount} products.");

            if (prodCount < pagination)
            {
                foreach (var product in products)
                {
                    Console.WriteLine($"id: {product.Id} ean: {product.Ean} name: {product.Name}");
                }
            } else
            {
                var newProducts = products.Index().ToDictionary();
                int counter = 0;
                while (counter < prodCount)
                {
                    newProducts.TryGetValue(counter, out var product);
                    if (product == null)
                    {
                        Console.WriteLine("Error: Found null product.");
                    } else
                    {
                        Console.WriteLine($"id: {product.Id} ean: {product.Ean} name: {product.Name}");
                    }
                    if (counter != 0 && counter % pagination == 0)
                    {
                        Console.WriteLine("If you want to exit write exit. If you want to see more click enter or write anything.");
                        var userInput = Console.ReadLine();
                        if (userInput == "exit") {
                            return;
                        }
                    }
                    counter++;
                }
            }

            Console.WriteLine("End of products.");
        }
    }
}
