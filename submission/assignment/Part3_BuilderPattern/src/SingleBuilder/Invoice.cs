namespace Part3_BuilderPattern.SingleBuilder;

// Task 3.2 - the "flat" invoice with ~20 properties on one class.
// The only way to create it is through Invoice.Builder (constructor is private).
public sealed class Invoice
{
    // --- customer ---
    public string InvoiceId { get; private set; } = "";
    public string CustomerName { get; private set; } = "";
    public string CustomerEmail { get; private set; } = "";
    public string? CustomerPhone { get; private set; }

    // --- billing address ---
    public string BillingStreet { get; private set; } = "";
    public string BillingCity { get; private set; } = "";
    public string? BillingState { get; private set; }
    public string BillingZipCode { get; private set; } = "";
    public string BillingCountry { get; private set; } = "";

    // --- shipping address ---
    public string ShippingStreet { get; private set; } = "";
    public string ShippingCity { get; private set; } = "";
    public string? ShippingState { get; private set; }
    public string ShippingZipCode { get; private set; } = "";
    public string ShippingCountry { get; private set; } = "";

    // --- order / payment ---
    public DateOnly OrderDate { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string Currency { get; private set; } = "";
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }

    // calculated, never set by hand
    public decimal TotalAmount => SubTotal - DiscountAmount + TaxAmount;

    private Invoice() { }

    public override string ToString()
    {
        return
$@"Invoice {InvoiceId} ({OrderDate:yyyy-MM-dd})
  Customer : {CustomerName} <{CustomerEmail}> {CustomerPhone}
  Billing  : {BillingStreet}, {BillingCity}, {BillingState} {BillingZipCode}, {BillingCountry}
  Shipping : {ShippingStreet}, {ShippingCity}, {ShippingState} {ShippingZipCode}, {ShippingCountry}
  Payment  : {PaymentMethod} in {Currency}
  SubTotal {SubTotal:F2} - Discount {DiscountAmount:F2} + Tax {TaxAmount:F2} = TOTAL {TotalAmount:F2}";
    }

    // ============================================================
    //  Task 3.2 - one big fluent builder
    // ============================================================
    public sealed class Builder
    {
        private string? invoiceId, customerName, customerEmail, customerPhone;
        private string? billingStreet, billingCity, billingState, billingZip, billingCountry;
        private string? shippingStreet, shippingCity, shippingState, shippingZip, shippingCountry;
        private DateOnly? orderDate;
        private PaymentMethod? paymentMethod;
        private string? currency;
        private decimal? subTotal;
        private decimal discountAmount;  // optional, default 0
        private decimal taxAmount;       // optional, default 0
        private bool built;

        // ----- customer -----
        public Builder WithInvoiceId(string id) { invoiceId = id; return this; }
        public Builder WithCustomerName(string name) { customerName = name; return this; }
        public Builder WithCustomerEmail(string email) { customerEmail = email; return this; }
        public Builder WithCustomerPhone(string phone) { customerPhone = phone; return this; }

        // ----- billing -----
        public Builder WithBillingStreet(string v) { billingStreet = v; return this; }
        public Builder WithBillingCity(string v) { billingCity = v; return this; }
        public Builder WithBillingState(string v) { billingState = v; return this; }
        public Builder WithBillingZipCode(string v) { billingZip = v; return this; }
        public Builder WithBillingCountry(string v) { billingCountry = v; return this; }

        // ----- shipping (optional as a group: if skipped, ship to billing address) -----
        public Builder WithShippingStreet(string v) { shippingStreet = v; return this; }
        public Builder WithShippingCity(string v) { shippingCity = v; return this; }
        public Builder WithShippingState(string v) { shippingState = v; return this; }
        public Builder WithShippingZipCode(string v) { shippingZip = v; return this; }
        public Builder WithShippingCountry(string v) { shippingCountry = v; return this; }

        // ----- order / payment -----
        public Builder WithOrderDate(DateOnly date) { orderDate = date; return this; }
        public Builder WithPaymentMethod(PaymentMethod method) { paymentMethod = method; return this; }
        public Builder WithCurrency(string code) { currency = code; return this; }
        public Builder WithSubTotal(decimal amount) { subTotal = amount; return this; }
        public Builder WithDiscount(decimal amount) { discountAmount = amount; return this; }
        public Builder WithTax(decimal amount) { taxAmount = amount; return this; }

        public Invoice Build()
        {
            if (built)
                throw new InvalidOperationException("This builder was already used. Create a new one.");

            var errors = new List<string>();

            Require(invoiceId, "InvoiceId", errors);
            Require(customerName, "CustomerName", errors);
            Require(customerEmail, "CustomerEmail", errors);
            if (!string.IsNullOrWhiteSpace(customerEmail) && !customerEmail.Contains('@'))
                errors.Add("CustomerEmail is not a valid email");

            Require(billingStreet, "BillingStreet", errors);
            Require(billingCity, "BillingCity", errors);
            Require(billingZip, "BillingZipCode", errors);
            Require(billingCountry, "BillingCountry", errors);

            // shipping: all-or-nothing (a half shipping address is a bug)
            bool anyShipping = shippingStreet != null || shippingCity != null || shippingState != null
                               || shippingZip != null || shippingCountry != null;
            if (anyShipping)
            {
                Require(shippingStreet, "ShippingStreet", errors);
                Require(shippingCity, "ShippingCity", errors);
                Require(shippingZip, "ShippingZipCode", errors);
                Require(shippingCountry, "ShippingCountry", errors);
            }

            if (orderDate == null) errors.Add("OrderDate is required");
            if (paymentMethod == null) errors.Add("PaymentMethod is required");
            Require(currency, "Currency", errors);
            if (currency != null && currency.Trim().Length != 3)
                errors.Add("Currency must be a 3-letter code (e.g. EGP)");

            if (subTotal == null) errors.Add("SubTotal is required");
            else if (subTotal < 0) errors.Add("SubTotal can't be negative");
            if (discountAmount < 0) errors.Add("DiscountAmount can't be negative");
            if (subTotal != null && discountAmount > subTotal) errors.Add("DiscountAmount can't be more than SubTotal");
            if (taxAmount < 0) errors.Add("TaxAmount can't be negative");

            if (errors.Count > 0)
                throw new InvalidOperationException("Can't build Invoice:\n - " + string.Join("\n - ", errors));

            built = true;
            return new Invoice
            {
                InvoiceId = invoiceId!,
                CustomerName = customerName!,
                CustomerEmail = customerEmail!,
                CustomerPhone = customerPhone,

                BillingStreet = billingStreet!,
                BillingCity = billingCity!,
                BillingState = billingState,
                BillingZipCode = billingZip!,
                BillingCountry = billingCountry!,

                ShippingStreet = anyShipping ? shippingStreet! : billingStreet!,
                ShippingCity = anyShipping ? shippingCity! : billingCity!,
                ShippingState = anyShipping ? shippingState : billingState,
                ShippingZipCode = anyShipping ? shippingZip! : billingZip!,
                ShippingCountry = anyShipping ? shippingCountry! : billingCountry!,

                OrderDate = orderDate!.Value,
                PaymentMethod = paymentMethod!.Value,
                Currency = currency!.Trim().ToUpperInvariant(),
                SubTotal = subTotal!.Value,
                DiscountAmount = discountAmount,
                TaxAmount = taxAmount
            };
        }

        private static void Require(string? value, string name, List<string> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{name} is required");
        }
    }
}