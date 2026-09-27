namespace Part2_HotelReservationSystem;

public class Guest
{
    // private backing field - only the guest adds to it
    private readonly List<Reservation> reservations = new();

    public int GuestId { get; }
    public string FullName { get; }
    public string PhoneNumber { get; }

    // AsReadOnly() returns a wrapper, so even casting it back to List<Reservation> fails.
    public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();

    public Guest(int guestId, string fullName, string phoneNumber)
    {
        if (guestId <= 0)
            throw new ArgumentException("Guest id must be positive.", nameof(guestId));
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

        GuestId = guestId;
        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
    }

    // The guest's own action that creates a reservation and adds it to the history.
    public Reservation MakeReservation(int reservationId, Room room, DateOnly checkIn, DateOnly checkOut)
    {
        // All validation happens inside the Reservation constructor.
        // If it throws, nothing is added.
        var reservation = new Reservation(reservationId, this, room, checkIn, checkOut);
        reservations.Add(reservation);
        return reservation;
    }

    public override string ToString() => $"Guest #{GuestId}: {FullName} ({PhoneNumber})";
}