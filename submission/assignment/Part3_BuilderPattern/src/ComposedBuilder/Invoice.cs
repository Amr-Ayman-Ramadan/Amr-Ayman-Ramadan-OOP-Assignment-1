namespace Part3_BuilderPattern.ComposedBuilder;

// Task 3.3 - the same data, but grouped: customer info + 2 x Address + OrderDetails.
public sealed class Invoice
{
    public string InvoiceId { get; }
    public string CustomerName { get; }
    public string CustomerEmail { get; }
    public string? CustomerPhone { get; }
    public Address BillingAddress { get; }
    public Address ShippingAddress { get; }
    public OrderDetails Order { get; }

    public decimal TotalAmount => Order.TotalAmount;

    internal Invoice(string invoiceId, string customerName, string customerEmail, string? customerPhone,
                     Address billing, Address shipping, OrderDetails order)
    {
        InvoiceId = invoiceId;
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        CustomerPhone = customerPhone;
        BillingAddress = billing;
        ShippingAddress = shipping;
        Order = order;
    }

    public override string ToString() =>
$@"Invoice {InvoiceId}
  Customer : {CustomerName} <{CustomerEmail}> {CustomerPhone}
  Billing  : {BillingAddress}
  Shipping : {ShippingAddress}
  Order    : {Order}";
}

// Parent builder: only knows invoice-level fields. It does NOT know the rules of a
// valid address or valid money amounts - the small builders do.
public sealed class InvoiceBuilder
{
    private string? invoiceId, customerName, customerEmail, customerPhone;
    private Address? billing, shipping;
    private OrderDetails? order;

    public InvoiceBuilder WithId(string id) { invoiceId = id; return this; }

    public InvoiceBuilder ForCustomer(string name, string email, string? phone = null)
    {
        customerName = name;
        customerEmail = email;
        customerPhone = phone;
        return this;
    }

    // Option 1: configure the address inline with a lambda
    public InvoiceBuilder WithBillingAddress(Action<AddressBuilder> configure)
    {
        billing = BuildAddress(configure);
        return this;
    }

    public InvoiceBuilder WithShippingAddress(Action<AddressBuilder> configure)
    {
        shipping = BuildAddress(configure);
        return this;
    }

    // Option 2: pass an already-built address (e.g. the customer's saved address)
    public InvoiceBuilder WithBillingAddress(Address address)
    {
        billing = address ?? throw new ArgumentNullException(nameof(address));
        return this;
    }

    public InvoiceBuilder WithShippingAddress(Address address)
    {
        shipping = address ?? throw new ArgumentNullException(nameof(address));
        return this;
    }

    public InvoiceBuilder WithOrder(Action<OrderBuilder> configure)
    {
        var builder = new OrderBuilder();
        configure(builder);
        order = builder.Build();   // throws right here if the order info is invalid
        return this;
    }

    public Invoice Build()
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(invoiceId)) errors.Add("InvoiceId is required");
        if (string.IsNullOrWhiteSpace(customerName)) errors.Add("Customer name is required");
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains('@'))
            errors.Add("A valid customer email is required");
        if (billing == null) errors.Add("Billing address is required");
        if (order == null) errors.Add("Order details are required");

        if (errors.Count > 0)
            throw new InvalidOperationException("Can't build Invoice:\n - " + string.Join("\n - ", errors));

        // shipping is optional -> same as billing
        return new Invoice(invoiceId!, customerName!, customerEmail!, customerPhone,
                           billing!, shipping ?? billing!, order!);
    }

    private static Address BuildAddress(Action<AddressBuilder> configure)
    {
        var builder = new AddressBuilder();   // SAME builder class for billing and shipping
        configure(builder);
        return builder.Build();
    }
}