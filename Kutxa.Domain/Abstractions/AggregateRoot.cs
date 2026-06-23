namespace Kutxa.Domain.Abstractions;

public abstract class AggregateRoot
{
    public Guid Id { get; protected set; }
}
