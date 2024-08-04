using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Dev2C2P.Services.Platform.Domain.Abstractions;

/// <summary>
/// This class is a base entity implementation of <see cref="IIdentifiable{TId}"/> interface.
/// **implement for EF.
/// </summary>
public abstract class Entity<TId, TUniqueId> : IIdentifiable<TId>
{
    [Key]
    public TId Id { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Entity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }

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
        if (obj == null || obj is not Entity<TId, TUniqueId>)
            return false;

        // Same instances must be considered as equal
        if (ReferenceEquals(this, obj))
            return true;

        var other = (Entity<TId, TUniqueId>)obj;

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
        where T : Entity<TId, TUniqueId>
    {
        return Id.Equals(other.Id);
    }
}
