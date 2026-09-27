namespace Part3_BuilderPattern.ComposedBuilder;

// Order + payment information of an invoice. Created only by OrderBuilder.
public sealed class OrderDetails
{
    public DateOnly OrderDate { get; }
    public PaymentMethod PaymentMethod { get; }
    public string Currency { get; }
    public decimal SubTotal { get; }
    public decimal DiscountAmount { get; }
    public decimal TaxAmount { get; }

    public decimal TotalAmount => SubTotal - DiscountAmount + TaxAmount;

    internal OrderDetails(DateOnly orderDate, PaymentMethod paymentMethod, string currency,
                          decimal subTotal, decimal discountAmount, decimal taxAmount)
    {
        OrderDate = orderDate;
        PaymentMethod = paymentMethod;
        Currency = currency;
        SubTotal = subTotal;
        DiscountAmount = discountAmount;
        TaxAmount = taxAmount;
    }

    public override string ToString() =>
        $"{OrderDate:yyyy-MM-dd}, {PaymentMethod}, SubTotal {SubTotal:F2} - Discount {DiscountAmount:F2} " +
        $"+ Tax {TaxAmount:F2} = TOTAL {TotalAmount:F2} {Currency}";
}

// Owns ONLY the money / payment rules.
public sealed class OrderBuilder
{
    private DateOnly? orderDate;
    private PaymentMethod? paymentMethod;
    private string? currency;
    private decimal? subTotal;
    private decimal discount;
    private decimal tax;

    public OrderBuilder OnDate(DateOnly date) { orderDate = date; return this; }
    public OrderBuilder PaidWith(PaymentMethod method) { paymentMethod = method; return this; }
    public OrderBuilder InCurrency(string code) { currency = code; return this; }
    public OrderBuilder SubTotal(decimal amount) { subTotal = amount; return this; }
    public OrderBuilder Discount(decimal amount) { discount = amount; return this; }
    public OrderBuilder Tax(decimal amount) { tax = amount; return this; }

    public OrderDetails Build()
    {
        var errors = new List<string>();
        if (orderDate == null) errors.Add("OrderDate is required");
        if (paymentMethod == null) errors.Add("PaymentMethod is required");
        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            errors.Add("Currency must be a 3-letter code (e.g. EGP)");
        if (subTotal == null) errors.Add("SubTotal is required");
        else if (subTotal < 0) errors.Add("SubTotal can't be negative");
        if (discount < 0) errors.Add("Discount can't be negative");
        if (subTotal != null && discount > subTotal) errors.Add("Discount can't be more than SubTotal");
        if (tax < 0) errors.Add("Tax can't be negative");

        if (errors.Count > 0)
            throw new InvalidOperationException("Invalid order details:\n - " + string.Join("\n - ", errors));

        return new OrderDetails(orderDate!.Value, paymentMethod!.Value, currency!.Trim().ToUpperInvariant(),
                                subTotal!.Value, discount, tax);
    }
}