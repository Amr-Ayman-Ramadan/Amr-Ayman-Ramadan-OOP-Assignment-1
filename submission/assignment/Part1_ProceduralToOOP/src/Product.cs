namespace Part1_ProceduralToOOP;

public class Product
{
    public int Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Stock { get; private set; }

    public Product(int id, string name, decimal price, int stock)
    {
        if (id <= 0)
            throw new ArgumentException("Product id must be positive.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.");
        if (price < 0)
            throw new ArgumentException("Product price can't be negative.");
        if (stock < 0)
            throw new ArgumentException("Product stock can't be negative.");

        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    public bool HasStock(int quantity) => Stock >= quantity;

    // Only the product itself changes its stock.
    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");
        if (!HasStock(quantity))
            throw new InvalidOperationException($"Not enough stock for product #{Id}.");

        Stock -= quantity;
    }

    public override string ToString()
    {
        return $"#{Id}  {Name}  price={Price:F2}  stock={Stock}";
    }
}