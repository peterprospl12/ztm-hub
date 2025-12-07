using ZtmHub.Domain.Exceptions;

namespace ZtmHub.Domain.ValueObjects;

public class PasswordHash : ValueObject
{
    public string Value { get; }

    public PasswordHash(string hashedValue)
    {
        if (string.IsNullOrWhiteSpace(hashedValue))
            throw new InvalidPasswordHashException();
        Value = hashedValue;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}