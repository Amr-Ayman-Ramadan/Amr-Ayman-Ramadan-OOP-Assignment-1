namespace Part2_HotelReservationSystem;

// Keeps the lists of guests and rooms and makes sure ids are unique.
public class Hotel
{
    private readonly Dictionary<int, Guest> guests = new();
    private readonly Dictionary<int, Room> rooms = new();
    private int nextReservationId = 1;

    public string Name { get; }

    public IReadOnlyCollection<Guest> Guests => guests.Values;
    public IReadOnlyCollection<Room> Rooms => rooms.Values;

    public Hotel(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Hotel name is required.", nameof(name));
        Name = name;
    }

    public Guest RegisterGuest(int guestId, string fullName, string phoneNumber)
    {
        if (guests.ContainsKey(guestId))
            throw new InvalidOperationException($"Guest id {guestId} already exists.");

        var guest = new Guest(guestId, fullName, phoneNumber);
        guests.Add(guestId, guest);
        return guest;
    }

    public Room AddRoom(int roomNumber, RoomType type, decimal nightlyRate)
    {
        if (rooms.ContainsKey(roomNumber))
            throw new InvalidOperationException($"Room {roomNumber} already exists.");

        var room = new Room(roomNumber, type, nightlyRate);
        rooms.Add(roomNumber, room);
        return room;
    }

    public Reservation Book(int guestId, int roomNumber, DateOnly checkIn, DateOnly checkOut)
    {
        var guest = GetGuest(guestId);
        var room = GetRoom(roomNumber);

        // the hotel gives the unique id, the guest makes the reservation
        var reservation = guest.MakeReservation(nextReservationId, room, checkIn, checkOut);
        nextReservationId++; // only increase if the booking succeeded
        return reservation;
    }

    public Guest GetGuest(int guestId) =>
        guests.TryGetValue(guestId, out var g) ? g
            : throw new KeyNotFoundException($"Guest {guestId} not found.");

    public Room GetRoom(int roomNumber) =>
        rooms.TryGetValue(roomNumber, out var r) ? r
            : throw new KeyNotFoundException($"Room {roomNumber} not found.");
}