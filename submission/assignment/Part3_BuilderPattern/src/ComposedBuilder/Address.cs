namespace Part3_BuilderPattern.ComposedBuilder;

// A complete, valid address. Can only be created by AddressBuilder.
// Used for BOTH billing and shipping.
public sealed class Address
{
    public string Street { get; }
    public string City { get; }
    public string? State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    internal Address(string street, string city, string? state, string zipCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
    }

    public override string ToString()
    {
        string state = string.IsNullOrWhiteSpace(State) ? "" : $", {State}";
        return $"{Street}, {City}{state} {ZipCode}, {Country}";
    }
}

// Owns ONLY the rules of "what is a complete address".
public sealed class AddressBuilder
{
    private string? street, city, state, zipCode, country;

    public AddressBuilder Street(string value) { street = value; return this; }
    public AddressBuilder City(string value) { city = value; return this; }
    public AddressBuilder State(string value) { state = value; return this; }
    public AddressBuilder ZipCode(string value) { zipCode = value; return this; }
    public AddressBuilder Country(string value) { country = value; return this; }

    public Address Build()
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(street)) missing.Add("Street");
        if (string.IsNullOrWhiteSpace(city)) missing.Add("City");
        if (string.IsNullOrWhiteSpace(zipCode)) missing.Add("ZipCode");
        if (string.IsNullOrWhiteSpace(country)) missing.Add("Country");

        if (missing.Count > 0)
            throw new InvalidOperationException("Incomplete address, missing: " + string.Join(", ", missing));

        if (!zipCode!.All(char.IsLetterOrDigit))
            throw new InvalidOperationException($"Zip code '{zipCode}' must contain only letters/digits.");

        return new Address(street!.Trim(), city!.Trim(), state?.Trim(), zipCode.Trim(), country!.Trim());
    }
}