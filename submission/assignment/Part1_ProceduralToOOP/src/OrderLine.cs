namespace Part1_ProceduralToOOP;

public class OrderLine
{
    public Product Product { get; }
    public int Quantity { get; }

    // Price is copied when the line is created, so if the product price
    // changes later, old orders don't change.
    public decimal UnitPrice { get; }

    public decimal LineTotal => UnitPrice * Quantity;

    public OrderLine(Product product, int quantity)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        Quantity = quantity;
        UnitPrice = product.Price;
    }

    public override string ToString()
    {
        return $"  - {Product.Name}  x{Quantity}  @{UnitPrice:F2}  = {LineTotal:F2}";
    }
}