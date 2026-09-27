# \# Assignment 5 — OOP Design, Encapsulation \& the Builder Pattern

# 

# \- \*\*Name:\*\* Amr Ayman Ramadan

# \- \*\*Email:\*\* amraymanramadan37@gmail.com

# \- \*\*Program:\*\* Advanced .NET Engineering Diploma — Group 2

# \## Structure

# 

# | Folder | Content |

# |---|---|

# | `submission/assignment/Part1\_ProceduralToOOP/` | `Critique.md` + C# console app replacing `order\_system.cpp` |

# | `submission/assignment/Part2\_HotelReservationSystem/` | Hotel reservation system designed from the manager's requirements |

# | `submission/assignment/Part3\_BuilderPattern/` | `Answers.md` + single builder (`src/SingleBuilder`) and composed builders (`src/ComposedBuilder`) |

# | `submission/assignment/Part4\_LeetCode/1679\_MaxNumberOfKSumPairs/` | Two-pointer solution + Accepted screenshot |

# | `submission/leetcode/account.md` | LeetCode account info |

# 

# Open `Assignment5.slnx` in Visual Studio, or run a part with

# `dotnet run --project submission/assignment/<PartFolder>/src`.

# 

# \## Notes \& assumptions

# 

# \### Part 1

# \- All state is owned by a `Store` object; no global/static mutable state.

# \- Every feature of the C++ program is kept (seed data, demo, menu options 1–8, paid sales total = 565.00 after demo).

# &#x20; Added menu options 9 (add customer) and 10 (add product).

# \- `OrderLine` stores the unit price at the time it was added, so later price changes don't affect paid orders.

# \- Paying an already-paid order is rejected. Money uses `decimal`. Invalid input no longer causes an infinite loop.

# 

# \### Part 2

# \- Date validity and status transitions → `Reservation`; positive rate and maintenance → `Room`;

# &#x20; non-empty name/phone → `Guest` constructor.

# \- \*\*No double booking is enforced by `Room`\*\*: the room is the resource being booked and owns its list of

# &#x20; reservations. The `Reservation` constructor calls `room.EnsureCanBeBooked(...)`, so an invalid reservation

# &#x20; is never created.

# \- Reservations are created only through `Guest.MakeReservation(...)` (the `Reservation` constructor is `internal`).

# \- Dates are treated as \[check-in, check-out): a guest can check in on the day the previous guest checks out.

# &#x20; Cancelled and checked-out reservations don't block the room.

# \- `TotalCost` = nights × current `Room.NightlyRate` (computed property).

# 

# \### Part 3

# \- If no shipping address is given, the billing address is used. `TotalAmount` is always computed.

# 

# \### Part 4

# \- Sort + two pointers, O(n log n) time, O(1) extra space. Accepted on LeetCode (51/51 test cases).

