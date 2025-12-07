using ZtmHub.Domain.Exceptions;

namespace ZtmHub.Domain.ValueObjects;

public class StopId : ValueObject
{
    public int Value { get; }
    
    public StopId(int value)
    {
        if (value <= 0) 
            throw new InvalidStopIdException("StopId must be greater than zero");
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}