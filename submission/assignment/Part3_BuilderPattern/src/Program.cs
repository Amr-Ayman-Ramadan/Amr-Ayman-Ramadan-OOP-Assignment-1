using Part3_BuilderPattern;
using Flat = Part3_BuilderPattern.SingleBuilder;
using Composed = Part3_BuilderPattern.ComposedBuilder;

// ==========================================================
// Task 3.2 - single big builder
// ==========================================================
Title("Task 3.2 - Single builder");

var invoice1 = new Flat.Invoice.Builder()
    .WithInvoiceId("INV-2026-0001")
    .WithCustomerName("Mona Ali")
    .WithCustomerEmail("mona@example.com")
    .WithCustomerPhone("01001234567")
    .WithBillingStreet("12 Tahrir St")
    .WithBillingCity("Cairo")
    .WithBillingZipCode("11511")
    .WithBillingCountry("Egypt")
    .WithShippingStreet("5 Corniche Rd")
    .WithShippingCity("Alexandria")
    .WithShippingZipCode("21500")
    .WithShippingCountry("Egypt")
    .WithOrderDate(new DateOnly(2026, 9, 20))
    .WithPaymentMethod(PaymentMethod.CreditCard)
    .WithCurrency("EGP")
    .WithSubTotal(1500m)
    .WithDiscount(150m)
    .WithTax(189m)
    .Build();

Console.WriteLine(invoice1);

Try("Forget mandatory fields (email, billing city, subtotal)", () =>
    new Flat.Invoice.Builder()
        .WithInvoiceId("INV-2026-0002")
        .WithCustomerName("Omar Hassan")
        .WithBillingStreet("1 Main St")
        .WithBillingZipCode("12345")
        .WithBillingCountry("Egypt")
        .WithOrderDate(new DateOnly(2026, 9, 21))
        .WithPaymentMethod(PaymentMethod.Cash)
        .WithCurrency("EGP")
        .Build());

Try("Half a shipping address", () =>
    new Flat.Invoice.Builder()
        .WithInvoiceId("INV-2026-0003")
        .WithCustomerName("Sara Nabil")
        .WithCustomerEmail("sara@example.com")
        .WithBillingStreet("3 Pyramids Rd").WithBillingCity("Giza")
        .WithBillingZipCode("12511").WithBillingCountry("Egypt")
        .WithShippingCity("Luxor")          // only the city!
        .WithOrderDate(new DateOnly(2026, 9, 22))
        .WithPaymentMethod(PaymentMethod.Wallet)
        .WithCurrency("EGP")
        .WithSubTotal(300m)
        .Build());

// ==========================================================
// Task 3.3 - composed builders
// ==========================================================
Title("Task 3.3 - Composed builders");

var invoice2 = new Composed.InvoiceBuilder()
    .WithId("INV-2026-0001")
    .ForCustomer("Mona Ali", "mona@example.com", "01001234567")
    .WithBillingAddress(a => a
        .Street("12 Tahrir St")
        .City("Cairo")
        .ZipCode("11511")
        .Country("Egypt"))
    .WithShippingAddress(a => a
        .Street("5 Corniche Rd")
        .City("Alexandria")
        .ZipCode("21500")
        .Country("Egypt"))
    .WithOrder(o => o
        .OnDate(new DateOnly(2026, 9, 20))
        .PaidWith(PaymentMethod.CreditCard)
        .InCurrency("EGP")
        .SubTotal(1500m)
        .Discount(150m)
        .Tax(189m))
    .Build();

Console.WriteLine(invoice2);

// Reuse: one saved address used as billing AND shipping (and for other invoices)
var savedAddress = new Composed.AddressBuilder()
    .Street("3 Pyramids Rd").City("Giza").ZipCode("12511").Country("Egypt")
    .Build();

var invoice3 = new Composed.InvoiceBuilder()
    .WithId("INV-2026-0004")
    .ForCustomer("Sara Nabil", "sara@example.com")
    .WithBillingAddress(savedAddress)       // no shipping -> ships to billing
    .WithOrder(o => o.OnDate(new DateOnly(2026, 9, 23)).PaidWith(PaymentMethod.Cash)
                     .InCurrency("egp").SubTotal(250m))
    .Build();

Console.WriteLine();
Console.WriteLine(invoice3);

Try("Incomplete address (the AddressBuilder rejects it by itself)", () =>
    new Composed.InvoiceBuilder()
        .WithId("INV-X")
        .ForCustomer("Test", "test@example.com")
        .WithBillingAddress(a => a.Street("Somewhere").Country("Egypt"))
        .Build());

Try("Discount bigger than subtotal (the OrderBuilder rejects it by itself)", () =>
    new Composed.InvoiceBuilder()
        .WithId("INV-Y")
        .ForCustomer("Test", "test@example.com")
        .WithBillingAddress(savedAddress)
        .WithOrder(o => o.OnDate(new DateOnly(2026, 9, 24)).PaidWith(PaymentMethod.Cash)
                         .InCurrency("EGP").SubTotal(100m).Discount(500m))
        .Build());

Try("No order details at all", () =>
    new Composed.InvoiceBuilder()
        .WithId("INV-Z")
        .ForCustomer("Test", "test@example.com")
        .WithBillingAddress(savedAddress)
        .Build());


static void Title(string text)
{
    Console.WriteLine();
    Console.WriteLine($"=== {text} ===");
}

static void Try(string description, Action action)
{
    try
    {
        action();
        Console.WriteLine($"[NOT REJECTED!] {description}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n[rejected] {description}\n{ex.Message}");
    }
}