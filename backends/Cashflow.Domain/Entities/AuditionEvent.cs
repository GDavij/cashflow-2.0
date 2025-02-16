using Cashflow.Core;

namespace Cashflow.Domain.Entities;

public class AuditionEvent : ValueObject<long>, IEvent
{
    public Guid TraceIdentifier { get; private set; }
    public required string @Event { get; init; }
    public long UserId { get; private set; }
    public User? User { get; init; }
    public DateTime OccuredAt { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public bool Private { get; init; }

    public void BindToUserWithId(long userId) => UserId = userId;
    
    public void BindToTrace(Guid traceId) => TraceIdentifier = traceId;

    public string Description() => Event;
}
