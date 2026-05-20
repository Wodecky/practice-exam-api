namespace PracticeExam.Domain.Common;

/// <summary>
/// Base type for domain entities. Entities are compared by identity.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
