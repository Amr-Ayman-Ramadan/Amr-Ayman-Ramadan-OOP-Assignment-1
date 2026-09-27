namespace Part1_ProceduralToOOP;

public class Customer
{
    // VIP customers get 10% off (was the magic number 0.90 in the C++ code)
    private const decimal VipDiscountRate = 0.10m;

    public int Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string City { get; }
    public bool IsVip { get; }

    public Customer(int id, string name, string email, string city, bool isVip)
    {
        if (id <= 0)
            throw new ArgumentException("Customer id must be positive.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Customer email is not valid.");

        Id = id;
        Name = name;
        Email = email;
        City = city ?? "";
        IsVip = isVip;
    }

    // The discount rule belongs to the customer, not to the order total function.
    public decimal ApplyDiscount(decimal amount)
    {
        return IsVip ? amount * (1 - VipDiscountRate) : amount;
    }

    public override string ToString()
    {
        return $"#{Id}  {Name}  <{Email}>  {City}  vip={(IsVip ? "yes" : "no")}";
    }
}