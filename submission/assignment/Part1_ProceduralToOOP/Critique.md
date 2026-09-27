\# Task 1.1 — Critique of `order\_system.cpp`



I built and ran the program first, went through the demo output and every menu option,

and then read the source top to bottom. These are the problems I found, roughly ordered

from most serious to least.



\---



\## 1. All the state is global



Every piece of data (`customerCount`, `productPrices`, `orderIsPaid`, ...) is a global

variable, and every function reads and writes it directly.



\*\*Why it's a problem:\*\* any function anywhere can change anything, so no single place

"owns" the data or protects it. To know who changes `productStock` I have to search the

whole file. You also can't have two independent stores (e.g. one for a unit test and one

for the real app) because there is only one set of globals, and a function can't be tested

alone because its result depends on hidden global state instead of its parameters.



\## 2. Parallel arrays instead of real objects



A "customer" doesn't exist as a thing. It's spread across five arrays (`customerIds`,

`customerNames`, `customerEmails`, `customerCities`, `customerIsVip`) connected only by

sharing the same index. Same for products, orders and order lines.



\*\*What can go wrong:\*\* if someone adds a new field (say `customerPhones`) and forgets to

fill it in `addCustomer`, the arrays go out of sync and customer #3 silently gets customer

\#2's phone. Sorting or deleting means moving the same index in five arrays at once — miss

one and the data is corrupted with no error. The compiler can't help, because to it these

are unrelated arrays.



\## 3. Fixed maximum sizes



`MAX\_CUSTOMERS = 50`, `MAX\_PRODUCTS = 50`, `MAX\_ORDERS = 100`, `MAX\_LINES\_PER\_ORDER = 20`.



\*\*Why it's a problem:\*\* arbitrary technical limits leak into the business ("sorry, the

shop can't have a 101st order"). The 2D arrays `lineProductIndexes\[100]\[20]` and

`lineQuantities\[100]\[20]` reserve space for 2000 lines even with 3 orders, and changing a

limit means recompiling.



\## 4. Relationships are stored as array indexes



Orders store `orderCustomerIndexes` and lines store `lineProductIndexes` — the position in

another array, not an ID and not a reference.



\*\*What can go wrong:\*\* as soon as someone adds "delete customer/product" or sorts the

arrays, every stored index points to the wrong record (or past the end). Order 1001 would

suddenly belong to another customer and nobody would notice. It also mixes two concepts:

some functions take an \*\*id\*\* (`printOrder(orderId)`) and others an \*\*index\*\*

(`calculateOrderTotal(orderIndex)`), both plain `int`s, so passing the wrong one compiles

and gives wrong results.



\## 5. Business rules are scattered across free functions



\- The VIP 10% discount is hard-coded in `calculateOrderTotal` (`total \* 0.90`).

\- "Paid orders can't be changed" lives in `addLineToOrder`.

\- "Can't pay an empty order" lives in `markOrderPaid`.

\- Stock checking and decrement happen in `addLineToOrder`, not next to the product data.



\*\*Why it's a problem:\*\* no class is responsible for "an order" or "a product", so when a

rule changes you have to hunt for it, and nothing stops new code from writing

`orderIsPaid\[i] = false` or `productStock\[i] = -5` directly and bypassing every rule.



\## 6. Missing validation



\- `addProduct` accepts a negative price or negative stock.

\- `addCustomer` accepts an empty name or email.

\- `createOrder` accepts any string as a date (`"banana"` works).

\- `markOrderPaid` doesn't check if the order is \*\*already\*\* paid, so paying twice silently

&#x20; "succeeds".



Because checks are written by hand in each function, it's easy to forget one — and some

were forgotten.



\## 7. Totals use the \*current\* product price



`calculateOrderTotal` and `printOrder` read `productPrices\[productIndex]` every time

instead of the price at the moment the line was added.



\*\*What can go wrong:\*\* if the USB cable price changes tomorrow, the total of an order that

was already \*\*paid\*\* changes too, and the "paid sales total" rewrites the past. An order

line should remember the unit price it was sold at.



\## 8. Error handling = print and return



Errors are reported with `cout << "ERROR: ..."` then `return;` (or `return -1`). Most

functions are `void`, so the caller can't know something failed, and `runDemoScenario`

ignores all results.



\*\*What can go wrong:\*\* if `createOrder` fails, every following `addLineToOrder` for that

id fails too, and the program keeps going in a broken state. It also mixes business logic

with console output, so the logic can't be reused in a GUI or web app.



\## 9. Logic and presentation are mixed



`printOrder` both calculates (line totals) and prints. The line-total formula is written

again inside `printOrder` instead of reusing one place — duplicated formulas drift apart.



\## 10. Money stored as `double`



Floating point can't represent values like `0.1` exactly, so after enough additions and

discounts totals can become `314.99999999`. Money should use a decimal type.



\## 11. Fragile input handling



`cin >> choice` has no checks: typing a letter puts `cin` into a fail state and the menu

loops forever printing "Unknown choice".



\## 12. Smaller issues



\- `createOrder` returns the internal index (`orderCount - 1`), leaking an implementation

&#x20; detail.

\- `using namespace std;` at global scope.

\- The menu can't add customers or products; you have to edit `seedSampleData`.

\- Magic number `0.90` instead of a named constant.



\---



\## How my C# version fixes these



| Problem | Fix in C# |

|---|---|

| Global state | A `Store` object owns all collections; `Program` creates one. No static mutable state. |

| Parallel arrays | Real classes: `Customer`, `Product`, `Order`, `OrderLine`. |

| Fixed sizes | `List<T>` / `Dictionary<int, T>`. |

| Index relationships | `Order` holds a `Customer` reference, `OrderLine` holds a `Product` reference. |

| Scattered rules | Each rule lives in the class that owns the data (`Product.RemoveStock`, `Order.AddLine`, `Order.MarkPaid`, `Customer.ApplyDiscount`). |

| Missing validation | Constructors and methods validate and throw exceptions; double payment is rejected. |

| Price changes affect old orders | `OrderLine` stores `UnitPrice` at creation time. |

| Print \& return errors | Domain classes throw; only `ConsoleMenu` catches and prints. |

| `double` money | `decimal`. |

| Bad input loops | `int.TryParse` / `DateOnly.TryParseExact` and ask again. |

| Can't add customers/products | Menu options 9 and 10. |

