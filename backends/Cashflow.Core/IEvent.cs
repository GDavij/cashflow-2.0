namespace Cashflow.Core;

public interface IEvent 
{
    public long Id { get; }
    public long UserId { get; }
    public Guid TraceIdentifier { get; }
    public DateTime OccuredAt { get; }
    public bool Private { get; }

    public void BindToUserWithId(long userId);
    public void BindToTrace(Guid traceId);

    public string Description();
}