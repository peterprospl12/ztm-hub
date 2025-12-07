using ZtmHub.Domain.Exceptions;

namespace ZtmHub.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidEmailException("Email cannot be null");
        if (!value.Contains("@") || !value.Contains("."))
            throw new InvalidEmailException("Invalid format of email");
        Value = value.ToLowerInvariant().Trim();
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}