
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
        public void CreateNewOrder();
        public void PassOrderToWarehouse();
        public void SendOrder();
        public void ShowOrders();
        public void ShowProducts();
        public void ModifyProduct();
    }
    public class ConsoleFunctions(IConsoleUtils consoleUtils, IOrderService orderService, IProductService productService) : IConsoleFunctions
    {
        private readonly IConsoleUtils _consoleUtils = consoleUtils;
        private readonly IOrderService _orderService = orderService;
        private readonly IProductService _productService = productService;
        private readonly string pricePattern = "^\\d\\.\\d{2}$";
        /// <summary>
        /// Try to receive parameters for user to create new order. Then write the outcome of action in console.
        /// </summary>
        public async void CreateNewOrder()
        {
            Console.WriteLine("Process of creating new order started. If you want to exit process write exit at any step.");
            bool wantToExit = false;

            string isCompanyText = "Is this order for company or physical person? Type 1 if yes or 0 if false.";
            wantToExit = _consoleUtils.GetParameter(isCompanyText, (a) => a != null && (a == "1" || a == "0"), out var isCompanyStr);
            if (wantToExit) return;

            string addressText = "Please write delivery address (max 250 characters).";
            wantToExit = _consoleUtils.GetParameter(addressText, (a) => (a ?? "").Length <= 250, out var address);
            if (wantToExit) return;

            var paymentList = await _orderService.GetPaymentOptions();
            var paymentInfoInString = paymentList.Select(x => x.Id + " - " + x.Name);
            string paymentText = "Please choose the payment option, by writing its number.\\n" + String.Join("\\n", paymentInfoInString);
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

                wantToExit = _consoleUtils.GetParameter(priceText, (a) => Regex.IsMatch(a ?? "", pricePattern), out var priceStr);
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
            int statusId = await _orderService.GetStatusId("Nowe");
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

        public async void ShowProducts()
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
                        Console.WriteLine("If you want to exit write exit. If you want to see click enter or write anything.");
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
