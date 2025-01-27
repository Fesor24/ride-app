namespace Ridely.Domain.Abstractions;
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public long Id { get; set; }

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
    public void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
