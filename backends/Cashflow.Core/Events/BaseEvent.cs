using Cashflow.Core;

namespace Cashflow.Core.Events;

public abstract class BaseEvent : IEvent
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public Guid TraceIdentifier { get; set; }
    public DateTime OccuredAt { get; set; } = DateTime.UtcNow;
    public bool Private { get; set; }

    public BaseEvent(bool @private)
    {
        Private = @private;
    }

    public void BindToUserWithId(long userId) => UserId = userId;
    public void BindToTrace(Guid traceId) => TraceIdentifier = traceId;
    public abstract string Description();
}

