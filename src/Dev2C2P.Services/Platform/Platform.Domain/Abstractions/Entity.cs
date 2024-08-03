using System.ComponentModel.DataAnnotations;

namespace Dev2C2P.Services.Platform.Domain.Abstractions;

/// <summary>
/// This class is a base entity implementation of <see cref="IIdentifiable{TId}"/> interface.
/// **implement for EF.
/// </summary>
public abstract class Entity<TId, TUniqueId> : IIdentifiable<TId>
{
    [Key]
    public TId Id { get; }

    /// <summary>
    /// Get entity unique identifier.
    /// </summary>
    public abstract TUniqueId GetId();

    /// <summary>
    /// Set entity unique identifier.
    /// </summary>
    /// <param name="uniqueId">A unique id of this entity.</param>
    public abstract void SetId(TUniqueId uniqueId);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not Entity<TUniqueId>)
            return false;

        // Same instances must be considered as equal
        if (ReferenceEquals(this, obj))
            return true;

        var other = (Entity<TUniqueId>)obj;

        // Must have a IS-A relation of types or must be same type
        var typeOfThis = GetType();
        var typeOfOther = other.GetType();
        if (!(typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther)
          || typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis)))
        {
            return false;
        }

        return DoEquals(other);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    protected bool DoEquals<T>(T other)
        where T : Entity<TUniqueId>
    {
        return Id.Equals(other.Id);
    }
}
