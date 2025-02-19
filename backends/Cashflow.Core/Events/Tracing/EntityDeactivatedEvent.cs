namespace Cashflow.Core.Events.Tracing;

public class EntityDeactivatedEvent<T> : BaseEvent
    where T : OwnableEntity<T>
{
    private readonly T _entity;

    public EntityDeactivatedEvent(T entity) : base(false)
    {
        _entity = entity;
    }

    public override string Description()
    {
        return $"Entity {typeof(T).FullName} with Id {_entity.Id} has been deactivated at {DateTime.UtcNow:O}.";
    }
}
