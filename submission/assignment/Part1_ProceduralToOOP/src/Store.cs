namespace Part1_ProceduralToOOP;

/// <summary>
/// Owns all the data that used to be global variables in the C++ version.
/// </summary>
public class Store
{
    private readonly Dictionary<int, Customer> customers = new();
    private readonly Dictionary<int, Product> products = new();
    private readonly Dictionary<int, Order> orders = new();
    private readonly List<Order> orderList = new();   // keeps insertion order for printing

    public IReadOnlyCollection<Customer> Customers => customers.Values;
    public IReadOnlyCollection<Product> Products => products.Values;
    public IReadOnlyList<Order> Orders => orderList;

    public Customer AddCustomer(int id, string name, string email, string city, bool isVip)
    {
        if (customers.ContainsKey(id))
            throw new InvalidOperationException($"Customer id {id} already exists.");

        var customer = new Customer(id, name, email, city, isVip);
        customers.Add(id, customer);
        return customer;
    }

    public Product AddProduct(int id, string name, decimal price, int stock)
    {
        if (products.ContainsKey(id))
            throw new InvalidOperationException($"Product id {id} already exists.");

        var product = new Product(id, name, price, stock);
        products.Add(id, product);
        return product;
    }

    public Order CreateOrder(int orderId, int customerId, DateOnly date)
    {
        if (orders.ContainsKey(orderId))
            throw new InvalidOperationException($"Order id {orderId} already exists.");

        var order = new Order(orderId, GetCustomer(customerId), date);
        orders.Add(orderId, order);
        orderList.Add(order);
        return order;
    }

    public void AddLineToOrder(int orderId, int productId, int quantity)
    {
        GetOrder(orderId).AddLine(GetProduct(productId), quantity);
    }

    public void PayOrder(int orderId)
    {
        GetOrder(orderId).MarkPaid();
    }

    public decimal TotalPaidSales()
    {
        return orderList.Where(o => o.IsPaid).Sum(o => o.Total);
    }

    public Customer GetCustomer(int id) =>
        customers.TryGetValue(id, out var c) ? c
            : throw new KeyNotFoundException($"Customer id {id} not found.");

    public Product GetProduct(int id) =>
        products.TryGetValue(id, out var p) ? p
            : throw new KeyNotFoundException($"Product id {id} not found.");

    public Order GetOrder(int id) =>
        orders.TryGetValue(id, out var o) ? o
            : throw new KeyNotFoundException($"Order id {id} not found.");
}