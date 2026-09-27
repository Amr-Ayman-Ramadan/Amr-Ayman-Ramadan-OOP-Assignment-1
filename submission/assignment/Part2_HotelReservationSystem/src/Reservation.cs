namespace Part2_HotelReservationSystem;

public class Reservation
{
    // Allowed moves: Pending -> Confirmed -> CheckedIn -> CheckedOut
    //                Pending/Confirmed -> Cancelled
    private static readonly Dictionary<ReservationStatus, ReservationStatus[]> AllowedTransitions = new()
    {
        [ReservationStatus.Pending] = new[] { ReservationStatus.Confirmed, ReservationStatus.Cancelled },
        [ReservationStatus.Confirmed] = new[] { ReservationStatus.CheckedIn, ReservationStatus.Cancelled },
        [ReservationStatus.CheckedIn] = new[] { ReservationStatus.CheckedOut },
        [ReservationStatus.CheckedOut] = Array.Empty<ReservationStatus>(),
        [ReservationStatus.Cancelled] = Array.Empty<ReservationStatus>(),
    };

    public int ReservationId { get; }
    public DateOnly CheckInDate { get; }
    public DateOnly CheckOutDate { get; }
    public Room Room { get; }        // reference to the real Room object
    public Guest Guest { get; }
    public ReservationStatus Status { get; private set; }

    public int NumberOfNights => CheckOutDate.DayNumber - CheckInDate.DayNumber;

    // Computed every time - nobody can type a wrong total by hand.
    public decimal TotalCost => NumberOfNights * Room.NightlyRate;

    public bool IsActive =>
        Status != ReservationStatus.Cancelled && Status != ReservationStatus.CheckedOut;

    // internal: a reservation is only created through Guest.MakeReservation(...)
    internal Reservation(int reservationId, Guest guest, Room room, DateOnly checkInDate, DateOnly checkOutDate)
    {
        if (reservationId <= 0)
            throw new ArgumentException("Reservation id must be positive.", nameof(reservationId));
        if (guest is null)
            throw new ArgumentNullException(nameof(guest));
        if (room is null)
            throw new ArgumentNullException(nameof(room));
        if (checkOutDate <= checkInDate)
            throw new ArgumentException("Check-out date must be strictly after check-in date.");

        // Room checks maintenance + double booking. If it throws,
        // the constructor fails and no Reservation object exists.
        room.EnsureCanBeBooked(checkInDate, checkOutDate);

        ReservationId = reservationId;
        Guest = guest;
        Room = room;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Status = ReservationStatus.Pending;

        room.AddReservation(this);
    }

    public bool Overlaps(DateOnly otherCheckIn, DateOnly otherCheckOut)
    {
        // [a, b) and [c, d) overlap if a < d and c < b
        return CheckInDate < otherCheckOut && otherCheckIn < CheckOutDate;
    }

    // ---------- the only ways to change the status ----------

    public void Confirm() => MoveTo(ReservationStatus.Confirmed);

    public void CheckIn()
    {
        if (Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException(
                $"Reservation {ReservationId} can't be checked in because it is {Status}, not Confirmed.");

        MoveTo(ReservationStatus.CheckedIn);
    }

    public void CheckOut() => MoveTo(ReservationStatus.CheckedOut);

    public void Cancel() => MoveTo(ReservationStatus.Cancelled);

    private void MoveTo(ReservationStatus newStatus)
    {
        if (!AllowedTransitions[Status].Contains(newStatus))
            throw new InvalidOperationException(
                $"Reservation {ReservationId}: can't move from {Status} to {newStatus}.");

        Status = newStatus;
    }

    public override string ToString()
    {
        return $"Reservation #{ReservationId}: {Guest.FullName}, Room {Room.RoomNumber}, " +
               $"{CheckInDate:yyyy-MM-dd} -> {CheckOutDate:yyyy-MM-dd} ({NumberOfNights} nights), " +
               $"{Status}, total = {TotalCost:F2}";
    }
}