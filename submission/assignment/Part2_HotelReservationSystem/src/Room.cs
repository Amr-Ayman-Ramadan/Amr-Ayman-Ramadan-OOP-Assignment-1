namespace Part2_HotelReservationSystem;

public class Room
{
    // The room keeps track of its own bookings so it can answer
    // "am I free between these dates?" (no double-booking rule).
    private readonly List<Reservation> reservations = new();

    public int RoomNumber { get; }
    public RoomType RoomType { get; }
    public decimal NightlyRate { get; private set; }
    public bool IsUnderMaintenance { get; private set; }

    public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();

    public Room(int roomNumber, RoomType roomType, decimal nightlyRate)
    {
        if (roomNumber <= 0)
            throw new ArgumentException("Room number must be positive.", nameof(roomNumber));
        if (!Enum.IsDefined(roomType))
            throw new ArgumentException("Unknown room type.", nameof(roomType));

        RoomNumber = roomNumber;
        RoomType = roomType;
        ChangeNightlyRate(nightlyRate); // same validation as the pricing action
        IsUnderMaintenance = false;
    }

    // ---------- pricing ----------

    public void ChangeNightlyRate(decimal newRate)
    {
        if (newRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(newRate), "Nightly rate must be a positive value.");

        NightlyRate = newRate;
    }

    // ---------- maintenance ----------

    public void StartMaintenance()
    {
        if (IsUnderMaintenance)
            throw new InvalidOperationException($"Room {RoomNumber} is already under maintenance.");

        IsUnderMaintenance = true;
    }

    public void EndMaintenance()
    {
        if (!IsUnderMaintenance)
            throw new InvalidOperationException($"Room {RoomNumber} is not under maintenance.");

        IsUnderMaintenance = false;
    }

    // ---------- availability ----------

    // True if no active (not cancelled / not checked-out) reservation overlaps the range.
    // The check-out day is free for the next guest.
    public bool IsAvailable(DateOnly checkIn, DateOnly checkOut)
    {
        return !reservations.Any(r => r.IsActive && r.Overlaps(checkIn, checkOut));
    }

    // Called by the Reservation constructor. Throws if the room can't be booked,
    // so an invalid reservation never gets created.
    internal void EnsureCanBeBooked(DateOnly checkIn, DateOnly checkOut)
    {
        if (IsUnderMaintenance)
            throw new InvalidOperationException($"Room {RoomNumber} is under maintenance and can't be booked.");

        if (!IsAvailable(checkIn, checkOut))
            throw new InvalidOperationException(
                $"Room {RoomNumber} is already booked between {checkIn:yyyy-MM-dd} and {checkOut:yyyy-MM-dd}.");
    }

    internal void AddReservation(Reservation reservation)
    {
        reservations.Add(reservation);
    }

    public override string ToString()
    {
        string state = IsUnderMaintenance ? " [MAINTENANCE]" : "";
        return $"Room {RoomNumber} ({RoomType}) - {NightlyRate:F2}/night{state}";
    }
}