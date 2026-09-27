using System.Globalization;
using Part2_HotelReservationSystem;

var hotel = new Hotel("Nile View Hotel");

// ---------- setup ----------
hotel.AddRoom(101, RoomType.Single, 800m);
var room202 = hotel.AddRoom(202, RoomType.Double, 1200m);
var room301 = hotel.AddRoom(301, RoomType.Suite, 3000m);

var ahmed = hotel.RegisterGuest(1, "Ahmed Mostafa", "01001234567");
var laila = hotel.RegisterGuest(2, "Laila Samir", "01119876543");

Title("Rooms");
foreach (var room in hotel.Rooms)
    Console.WriteLine(room);

// ---------- happy path ----------
Title("Normal flow: Pending -> Confirmed -> CheckedIn -> CheckedOut");
var r1 = hotel.Book(ahmed.GuestId, 101, D("2026-10-01"), D("2026-10-04"));
Console.WriteLine(r1);
r1.Confirm();
r1.CheckIn();
r1.CheckOut();
Console.WriteLine(r1);

var r2 = hotel.Book(ahmed.GuestId, 301, D("2026-11-10"), D("2026-11-12"));
r2.Confirm();

Title($"{ahmed.FullName}'s reservation history");
foreach (var r in ahmed.Reservations)
    Console.WriteLine(r);

// ---------- pricing ----------
Title("Pricing");
room301.ChangeNightlyRate(3500m);
Console.WriteLine($"New suite rate -> {r2.TotalCost:F2} for {r2.NumberOfNights} nights");
Try("Set rate to 0", () => room301.ChangeNightlyRate(0));
Try("Set rate to -100", () => room301.ChangeNightlyRate(-100));

// ---------- business rules ----------
Title("Business rules (all of these must be rejected)");

Try("Check-out before check-in",
    () => hotel.Book(laila.GuestId, 202, D("2026-10-10"), D("2026-10-08")));

Try("Check-out on the same day as check-in",
    () => hotel.Book(laila.GuestId, 202, D("2026-10-10"), D("2026-10-10")));

room202.StartMaintenance();
Try("Book a room under maintenance",
    () => hotel.Book(laila.GuestId, 202, D("2026-10-10"), D("2026-10-12")));
room202.EndMaintenance();
Console.WriteLine("Maintenance ended, booking room 202 again...");
var r3 = hotel.Book(laila.GuestId, 202, D("2026-10-10"), D("2026-10-12"));
Console.WriteLine(r3);

Try("Double booking (overlapping dates on room 202)",
    () => hotel.Book(ahmed.GuestId, 202, D("2026-10-11"), D("2026-10-14")));

Console.WriteLine("Booking room 202 starting on r3's check-out day (no overlap):");
var r4 = hotel.Book(ahmed.GuestId, 202, D("2026-10-12"), D("2026-10-13"));
Console.WriteLine(r4);

Try("Check in a Pending reservation", () => r3.CheckIn());
Try("CheckedOut -> CheckedIn", () => r1.CheckIn());
Try("Cancel a checked-out reservation", () => r1.Cancel());

r4.Cancel();
Console.WriteLine($"r4 cancelled -> {r4.Status}");
Try("Confirm a cancelled reservation", () => r4.Confirm());

Console.WriteLine("After cancelling r4, its dates are free again:");
var r5 = hotel.Book(laila.GuestId, 202, D("2026-10-12"), D("2026-10-13"));
Console.WriteLine(r5);

Try("Guest with empty name", () => new Guest(10, "  ", "0100"));
Try("Guest with empty phone", () => new Guest(11, "Someone", ""));
Try("Duplicate guest id", () => hotel.RegisterGuest(1, "Copy", "0100"));

// Encapsulation check: this line would NOT compile, because Reservations is IReadOnlyList:
//   ahmed.Reservations.Add(r5);
// and casting it back to List fails at runtime because it's a ReadOnlyCollection wrapper:
Try("Cast Reservations back to List and add", () =>
{
    var list = (List<Reservation>)ahmed.Reservations;
    list.Add(r5);
});

Title("Final state");
foreach (var guest in hotel.Guests)
{
    Console.WriteLine(guest);
    foreach (var r in guest.Reservations)
        Console.WriteLine("   " + r);
}


// ---------- helpers ----------
static DateOnly D(string s) => DateOnly.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture);

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
        Console.WriteLine($"[rejected] {description} -> {ex.GetType().Name}: {ex.Message}");
    }
}