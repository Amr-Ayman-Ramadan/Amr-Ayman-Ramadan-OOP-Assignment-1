using System.Text;

namespace Part1_ProceduralToOOP;

public class Order
{
    private readonly List<OrderLine> lines = new();

    public int Id { get; }
    public Customer Customer { get; }   // real reference, not an array index
    public DateOnly Date { get; }
    public bool IsPaid { get; private set; }

    public IReadOnlyList<OrderLine> Lines => lines;

    public Order(int id, Customer customer, DateOnly date)
    {
        if (id <= 0)
            throw new ArgumentException("Order id must be positive.");

        Id = id;
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        Date = date;
        IsPaid = false;
    }

    public void AddLine(Product product, int quantity)
    {
        if (IsPaid)
            throw new InvalidOperationException("Cannot change a paid order.");
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.");

        product.RemoveStock(quantity);   // Product checks and reduces its own stock
        lines.Add(new OrderLine(product, quantity));
    }

    public void MarkPaid()
    {
        if (IsPaid)
            throw new InvalidOperationException($"Order #{Id} is already paid.");
        if (lines.Count == 0)
            throw new InvalidOperationException("Cannot pay an empty order.");

        IsPaid = true;
    }

    public decimal SubTotal => lines.Sum(l => l.LineTotal);

    public decimal Total => Customer.ApplyDiscount(SubTotal);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== ORDER #{Id} ===");
        sb.AppendLine($"Date: {Date:yyyy-MM-dd}");
        sb.AppendLine($"Customer: {Customer.Name} (#{Customer.Id})");
        sb.AppendLine($"Paid: {(IsPaid ? "yes" : "no")}");
        sb.AppendLine("Lines:");
        foreach (var line in lines)
            sb.AppendLine(line.ToString());
        sb.Append($"TOTAL: {Total:F2}");
        return sb.ToString();
    }
}