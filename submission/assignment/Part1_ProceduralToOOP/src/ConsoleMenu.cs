using System.Globalization;

namespace Part1_ProceduralToOOP;

public class ConsoleMenu
{
    private readonly Store store;

    public ConsoleMenu(Store store)
    {
        this.store = store;
    }

    public void Run()
    {
        while (true)
        {
            PrintMenu();
            int choice = ReadInt("Choice: ");

            if (choice == 0)
            {
                Console.WriteLine("Bye.");
                return;
            }

            // All domain errors are exceptions -> caught in one place
            try
            {
                HandleChoice(choice);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or KeyNotFoundException)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
    }

    private void HandleChoice(int choice)
    {
        switch (choice)
        {
            case 1: PrintCustomers(); break;
            case 2: PrintProducts(); break;
            case 3: PrintAllOrders(); break;
            case 4:
                Console.WriteLine();
                Console.WriteLine(store.GetOrder(ReadInt("Order id: ")));
                break;
            case 5:
                {
                    int orderId = ReadInt("Order id: ");
                    int customerId = ReadInt("Customer id: ");
                    DateOnly date = ReadDate("Date (YYYY-MM-DD): ");
                    store.CreateOrder(orderId, customerId, date);
                    Console.WriteLine($"Order #{orderId} created.");
                    break;
                }
            case 6:
                {
                    int orderId = ReadInt("Order id: ");
                    int productId = ReadInt("Product id: ");
                    int quantity = ReadInt("Quantity: ");
                    store.AddLineToOrder(orderId, productId, quantity);
                    Console.WriteLine("Line added.");
                    break;
                }
            case 7:
                {
                    int orderId = ReadInt("Order id: ");
                    store.PayOrder(orderId);
                    Console.WriteLine($"Order #{orderId} marked as paid.");
                    break;
                }
            case 8:
                PrintPaidSalesTotal("Paid sales total");
                break;
            case 9:
                {
                    int id = ReadInt("Customer id: ");
                    string name = ReadText("Name: ");
                    string email = ReadText("Email: ");
                    string city = ReadText("City: ");
                    bool vip = ReadText("VIP? (y/n): ").Trim().ToLower() == "y";
                    store.AddCustomer(id, name, email, city, vip);
                    Console.WriteLine("Customer added.");
                    break;
                }
            case 10:
                {
                    int id = ReadInt("Product id: ");
                    string name = ReadText("Name: ");
                    decimal price = ReadDecimal("Price: ");
                    int stock = ReadInt("Stock: ");
                    store.AddProduct(id, name, price, stock);
                    Console.WriteLine("Product added.");
                    break;
                }
            default:
                Console.WriteLine("Unknown choice.");
                break;
        }
    }

    public void PrintCustomers()
    {
        Console.WriteLine($"\n=== CUSTOMERS ({store.Customers.Count}) ===");
        foreach (var c in store.Customers)
            Console.WriteLine(c);
    }

    public void PrintProducts()
    {
        Console.WriteLine($"\n=== PRODUCTS ({store.Products.Count}) ===");
        foreach (var p in store.Products)
            Console.WriteLine(p);
    }

    public void PrintAllOrders()
    {
        Console.WriteLine($"\n=== ALL ORDERS ({store.Orders.Count}) ===");
        foreach (var o in store.Orders)
        {
            Console.WriteLine();
            Console.WriteLine(o);
        }
    }

    public void PrintPaidSalesTotal(string label)
    {
        Console.WriteLine($"{label}: {store.TotalPaidSales():F2}");
    }

    private static void PrintMenu()
    {
        Console.WriteLine();
        Console.WriteLine("---------- MENU ----------");
        Console.WriteLine("1) Print customers");
        Console.WriteLine("2) Print products");
        Console.WriteLine("3) Print all orders");
        Console.WriteLine("4) Print one order by id");
        Console.WriteLine("5) Create order");
        Console.WriteLine("6) Add line to order");
        Console.WriteLine("7) Mark order paid");
        Console.WriteLine("8) Show paid sales total");
        Console.WriteLine("9) Add customer");
        Console.WriteLine("10) Add product");
        Console.WriteLine("0) Exit");
    }

    // ---------- input helpers (no more infinite loop on bad input) ----------

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        string? line = Console.ReadLine();
        if (line == null)
            Environment.Exit(0);
        return line;
    }

    private static int ReadInt(string prompt)
    {
        while (true)
        {
            if (int.TryParse(ReadText(prompt), out int value))
                return value;
            Console.WriteLine("Please enter a whole number.");
        }
    }

    private static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            if (decimal.TryParse(ReadText(prompt), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
                return value;
            Console.WriteLine("Please enter a number (e.g. 99.50).");
        }
    }

    private static DateOnly ReadDate(string prompt)
    {
        while (true)
        {
            if (DateOnly.TryParseExact(ReadText(prompt).Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateOnly date))
                return date;
            Console.WriteLine("Please use the format YYYY-MM-DD.");
        }
    }
}