using ConsoleOrderExecutor.ConsoleFunction;
using ConsoleOrderExecutor.ConsoleFunction.Utils;
using ConsoleOrderExecutor.context;
using ConsoleOrderExecutor.Orders.Services;
using ConsoleOrderExecutor.Products.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

try
{
    string currentPath = Directory.GetCurrentDirectory();
    int binIndex = currentPath.IndexOf("bin") - 1;
    currentPath = currentPath.Substring(0, binIndex);
    
    HostApplicationBuilder builder = Host.CreateApplicationBuilder();
    var config = builder.Configuration.SetBasePath(currentPath)
        .AddJsonFile("appsettings.json", false, true).Build();

    if (config["ConnectionString"] == null || config["ConnectionString"] == "")
    {
        throw new Exception("Error: Could not find connection string in appsettings.json file.");
    }

    builder.Services.AddDbContext<ConsoleOrderExecutorDbContext>();
    builder.Services.AddSingleton<IProductService, ProductService>();
    builder.Services.AddSingleton<IOrderService, OrderService>();
    builder.Services.AddSingleton<IConsoleUtils, ConsoleUtils>();
    builder.Services.AddSingleton<IConsoleFunctions, ConsoleFunctions>();
    builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
    builder.Logging.AddFilter("System", LogLevel.Warning);

    using IHost host = builder.Build();

    RunApp(host);

    await host.RunAsync();

    static async void RunApp(IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider serviceProvider = serviceScope.ServiceProvider;
        IConsoleFunctions? consoleFunctions = serviceProvider.GetService<IConsoleFunctions>() ?? throw new Exception("Could not load the console functions.");
        IConsoleUtils? consoleUtils = serviceProvider.GetService<IConsoleUtils>() ?? throw new Exception("Could not load the console utils.");

        string[] functions = { "1. Create order.", "2. Pass order to warehouse.", "3. Send order.", "4. Show orders.", "5. Show products.", "6. Modify products" };
        var functionsJoined = String.Join("\n", functions);

        Console.WriteLine("Welcome in ConsoleOrderExecutor app. Here's the list of operation that you can use.");
        Console.WriteLine(functionsJoined);
        Console.WriteLine("If you want to exit application write exit.");
        bool wantToExit = false;

        string prompText = "\nPlease write the number of the operation that you want to execute. If you want to display list of operation write help.";

        while (!wantToExit) {
            Console.WriteLine(prompText);
            string? userInput = Console.ReadLine();
            if (userInput == null)
            {
                Console.WriteLine("Error: Value cannot be empty.");
                continue;
            }
            if (userInput == "exit")
            {
                Console.WriteLine("Exiting the application...");
                await host.StopAsync();
                return;
            }
            if (userInput == "help")
            {
                Console.WriteLine("Here's the list of operation that you can use.");
                Console.WriteLine(functionsJoined);
                Console.WriteLine("If you want to exit application write exit.");
                continue;
            }
            if (Regex.IsMatch(userInput, "\\d+"))
            {
                int operationNumber = Int32.Parse(userInput);
                switch (operationNumber)
                {
                    case 1:
                        await consoleFunctions.CreateNewOrder();
                        break;
                    case 2:
                        await consoleFunctions.PassOrderToWarehouse();
                        break;
                    case 3:
                        await consoleFunctions.SendOrder();
                        break;
                    case 4:
                        await consoleFunctions.ShowOrders();
                        break;
                    case 5:
                        await consoleFunctions.ShowProducts();
                        break;
                    case 6:
                        await consoleFunctions.ModifyProduct();
                        break;
                    default:
                        Console.WriteLine("Error: The operation with this number do not exist.");
                        break;
                }
            } else
            {
                Console.WriteLine("Error: Invalid input. If you want to see the list of operation write help.");
            }
        }
    }

}
catch (Exception ex) { 
    Console.WriteLine(ex.ToString());
}