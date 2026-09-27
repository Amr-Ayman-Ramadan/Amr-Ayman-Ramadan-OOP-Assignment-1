using Part1_ProceduralToOOP;

Console.WriteLine("Order System (OOP version)");
Console.WriteLine("Seed sample data, show a demo, then open the menu.");

// The only "state" is this object - no globals anywhere.
var store = new Store();
SeedSampleData(store);
RunDemoScenario(store);

var menu = new ConsoleMenu(store);
menu.PrintCustomers();
menu.PrintProducts();
menu.PrintAllOrders();
Console.WriteLine();
menu.PrintPaidSalesTotal("Paid sales total after demo");

menu.Run();


static void SeedSampleData(Store store)
{
    store.AddCustomer(1, "Mona Ali", "mona@example.com", "Cairo", true);
    store.AddCustomer(2, "Omar Hassan", "omar@example.com", "Alexandria", false);
    store.AddCustomer(3, "Sara Nabil", "sara@example.com", "Giza", false);

    store.AddProduct(101, "USB Cable", 50.0m, 100);
    store.AddProduct(102, "Wireless Mouse", 250.0m, 40);
    store.AddProduct(103, "Mechanical Keyboard", 1200.0m, 15);
    store.AddProduct(104, "Laptop Stand", 400.0m, 25);
}

static void RunDemoScenario(Store store)
{
    var order1 = store.CreateOrder(1001, 1, new DateOnly(2026, 9, 15));
    order1.AddLine(store.GetProduct(101), 2);
    order1.AddLine(store.GetProduct(102), 1);
    order1.MarkPaid();

    var order2 = store.CreateOrder(1002, 2, new DateOnly(2026, 9, 15));
    order2.AddLine(store.GetProduct(103), 1);
    order2.AddLine(store.GetProduct(104), 1);

    var order3 = store.CreateOrder(1003, 3, new DateOnly(2026, 9, 16));
    order3.AddLine(store.GetProduct(101), 5);
    order3.MarkPaid();
}