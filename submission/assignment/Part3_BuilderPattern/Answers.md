\# Part 3 — Builder Pattern: Written Answers



The class I used is an `Invoice` with 21 pieces of data:



\- \*\*Customer:\*\* `InvoiceId`, `CustomerName`, `CustomerEmail`, `CustomerPhone`

\- \*\*Billing address:\*\* `BillingStreet`, `BillingCity`, `BillingState`, `BillingZipCode`, `BillingCountry`

\- \*\*Shipping address:\*\* `ShippingStreet`, `ShippingCity`, `ShippingState`, `ShippingZipCode`, `ShippingCountry`

\- \*\*Order / payment:\*\* `OrderDate`, `PaymentMethod`, `Currency`, `SubTotal`, `DiscountAmount`, `TaxAmount`, `TotalAmount`



(`TotalAmount` is calculated from the others, so nobody sets it.)



\---



\## Task 3.1 — Questions (answered before writing the code)



\### Q1. Why is a single 20-parameter constructor a problem in practice?



\*\*1. The call site is unreadable.\*\* A call looks like this:



```csharp

new Invoice("INV-1", "Mona Ali", "mona@example.com", null,

&#x20;           "12 Tahrir St", "Cairo", null, "11511", "Egypt",

&#x20;           "5 Corniche Rd", "Alexandria", null, "21500", "Egypt",

&#x20;           new DateOnly(2026, 9, 20), PaymentMethod.CreditCard, "EGP",

&#x20;           1500m, 150m, 189m);

```



Reading it, I can't tell what `null` #2 is, or which of `150m` and `189m` is the discount and which is the tax,

without opening the constructor. Code review can't catch mistakes it can't read.



\*\*2. Swapping values of the same type compiles fine.\*\* Most parameters are `string` and the last three are

`decimal`. If I pass `(..., 1500m, 189m, 150m)` — tax and discount swapped — the compiler is happy and the

invoice total is silently wrong (1500 − 189 + 150 instead of 1500 − 150 + 189). Same thing with billing vs.

shipping: both are five strings in a row, so swapping `"Cairo"` and `"Alexandria"` or a street and a city

ships the order to the wrong place. These are the worst kind of bugs: no error, just wrong data.



\*\*3. Optional values have to be passed anyway.\*\* Phone, state, discount and tax are optional, but with a

single constructor every caller must still pass something (`null`, `0m`), which adds noise and more chances

to put the `null` in the wrong slot. Optional parameters (`= null`) help a bit but then you get named

arguments everywhere, which is basically a builder without the validation.



\*\*4. Adding one more property breaks everyone.\*\* The day we add, e.g., `ShippingNotes`:

\- Either we change the constructor → \*\*every\*\* existing call in the codebase stops compiling and has to be

&#x20; edited, or

\- we add a second overload → now there are two 20-ish-parameter constructors ("telescoping constructors"),

&#x20; and it gets worse with every new field.

\- If the new parameter has the same type as its neighbour and is inserted in the middle, old calls might even

&#x20; still compile with the values shifted by one position — silently wrong again.



\*\*5. Validation is all-or-nothing in one place.\*\* All the rules for 20 values end up in one huge constructor.



\### Q2. Is it only "the constructor is too long", or is there a deeper design problem?



It's a deeper problem. The long constructor is a \*\*symptom\*\*; the cause is that one class has \~20 loosely

related properties that really belong to a few \*\*different concepts\*\*:



\- An \*\*address\*\* (street, city, state, zip, country) — and it appears \*\*twice\*\*, with the same fields and the

&#x20; same rules, just with a `Billing`/`Shipping` prefix. That duplication is a sign that "Address" is a missing

&#x20; class.

\- \*\*Order / payment info\*\* (date, method, currency, amounts) — has its own rules (amounts not negative,

&#x20; discount ≤ subtotal, 3-letter currency) that have nothing to do with addresses.

\- \*\*Customer info\*\* — which arguably belongs to a `Customer` object, not copied into every invoice.



Because everything is flat on one class, the `Invoice` has too many responsibilities (it must know the rules

of an address, the rules of money, and the rules of an invoice), the address validation must be written

twice, and you can't reuse "an address" anywhere else (e.g. a customer's saved address). Even with a perfect

builder, a flat 20-property class is still harder to understand than `Invoice { Customer, BillingAddress,

ShippingAddress, OrderDetails }`. So the Builder fixes \*construction\*, but splitting the data into smaller

types (Task 3.3) fixes the \*design\*.



\---



\## Task 3.2 — What I decided about mandatory vs optional



| Mandatory | Optional |

|---|---|

| InvoiceId, CustomerName, CustomerEmail | CustomerPhone |

| Billing: Street, City, ZipCode, Country | Billing/Shipping State (not every country has states) |

| OrderDate, PaymentMethod, Currency, SubTotal | Shipping address as a group (defaults to billing address) |

| | DiscountAmount, TaxAmount (default 0) |



`Build()` collects \*\*all\*\* missing/invalid fields and throws \*\*one\*\* `InvalidOperationException` that lists them,

so the developer sees everything that's wrong in one run instead of fixing one field at a time. The shipping

address is all-or-nothing: if you set any shipping field, you must set the whole address. `Invoice` has a

private constructor and private setters, so `Invoice.Builder` is the only way to create one, and a built

invoice can't be changed afterwards.



\---



\## Task 3.3 — Why the composed builders are better than the single big builder



In the composed version (`ComposedBuilder/` folder) the invoice is made of `Address` (×2) and `OrderDetails`,

built by `AddressBuilder` and `OrderBuilder`, and `InvoiceBuilder` just puts them together.



\### Single responsibility

\- \*\*`AddressBuilder`\*\* owns only "what is a complete, valid address": street, city, zip, country are required,

&#x20; zip must be letters/digits, state is optional. It knows nothing about money or invoices.

\- \*\*`OrderBuilder`\*\* owns only the order/payment rules: date and payment method required, 3-letter currency,

&#x20; no negative amounts, discount not bigger than subtotal. It knows nothing about addresses.

\- \*\*`InvoiceBuilder`\*\* owns only the invoice-level rules: there must be an id, a customer, a billing address and

&#x20; order details; shipping defaults to billing.



In the single builder, one class had 21 fields and one `Build()` method mixing all of these rules together

(about 40 lines of `if`s). If the zip-code rule changes, I have to edit the same method that also handles tax

rules — more chance of breaking something unrelated.



\### Independent validation

Yes — `AddressBuilder.Build()` either returns a \*\*complete\*\* `Address` or throws. `Address` has an `internal`

constructor, so an incomplete address can't exist. The parent `InvoiceBuilder` just checks "do I have an

Address?" (`billing == null`), and it doesn't need to know anything about streets, cities or zip rules. If we

later add a rule (e.g. Egyptian postal codes must be 5 digits), it's added in \*\*one\*\* place and every invoice,

billing and shipping, gets it automatically. Errors also show up earlier — at `.WithBillingAddress(...)`, with

a message that says exactly which part is wrong ("Incomplete address, missing: City, ZipCode").



\### Reuse

The same `AddressBuilder` (and the same `Address` type) is used for \*\*both\*\* billing and shipping. Without it

I had to duplicate:

\- 5 fields for billing + 5 almost identical fields for shipping in the builder,

\- 10 `WithBillingX` / `WithShippingX` methods that do the same thing,

\- the same 4 "required" checks twice in `Build()` (and it's easy to update one and forget the other),

\- 10 properties on `Invoice`.



Also, because an `Address` is now a real object, I can build it once (e.g. a customer's saved address) and pass

it to many invoices — or use it as both billing and shipping — see `savedAddress` in `Program.cs`. That wasn't

possible with loose strings.



\### Readability at the call site

Single builder (Task 3.2):

```csharp

new Invoice.Builder()

&#x20;   .WithInvoiceId("INV-2026-0001")

&#x20;   .WithCustomerName("Mona Ali")

&#x20;   .WithCustomerEmail("mona@example.com")

&#x20;   .WithBillingStreet("12 Tahrir St")

&#x20;   .WithBillingCity("Cairo")

&#x20;   .WithBillingZipCode("11511")

&#x20;   .WithBillingCountry("Egypt")

&#x20;   .WithShippingStreet("5 Corniche Rd")

&#x20;   .WithShippingCity("Alexandria")

&#x20;   .WithShippingZipCode("21500")

&#x20;   .WithShippingCountry("Egypt")

&#x20;   .WithOrderDate(new DateOnly(2026, 9, 20))

&#x20;   .WithPaymentMethod(PaymentMethod.CreditCard)

&#x20;   .WithCurrency("EGP")

&#x20;   .WithSubTotal(1500m).WithDiscount(150m).WithTax(189m)

&#x20;   .Build();

```

It's much better than the 20-parameter constructor (every value is named), but it's one long flat list; the

groups only exist because of the `Billing`/`Shipping` prefixes in the method names, and nothing stops you

from mixing a billing line in the middle of the shipping lines.



Composed builders (Task 3.3):

```csharp

new InvoiceBuilder()

&#x20;   .WithId("INV-2026-0001")

&#x20;   .ForCustomer("Mona Ali", "mona@example.com")

&#x20;   .WithBillingAddress(a => a.Street("12 Tahrir St").City("Cairo").ZipCode("11511").Country("Egypt"))

&#x20;   .WithShippingAddress(a => a.Street("5 Corniche Rd").City("Alexandria").ZipCode("21500").Country("Egypt"))

&#x20;   .WithOrder(o => o.OnDate(new DateOnly(2026, 9, 20)).PaidWith(PaymentMethod.CreditCard)

&#x20;                    .InCurrency("EGP").SubTotal(1500m).Discount(150m).Tax(189m))

&#x20;   .Build();

```

The \*\*structure of the code matches the structure of the data\*\*: one block per address, one block for the

order. Method names are shorter (`City` instead of `WithShippingCity`) because the context already tells you

which address you're in. You physically can't put a shipping city in the billing block. And the resulting

object is also easier to use: `invoice.ShippingAddress.City` instead of `invoice.ShippingCity`.



\*\*Trade-off (to be fair):\*\* the composed version has more classes (3 builders + 2 small types instead of 1

builder), and the lambda style (`a => a...`) is a bit unfamiliar at first. For a class with 4–5 fields this

would be over-engineering, but for \~20 fields with repeated groups it clearly pays off.

