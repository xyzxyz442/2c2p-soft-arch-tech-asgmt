namespace Dev2C2P.Services.Platform.Domain.Abstractions;

/// <summary>
/// Define interface for base entity type, All entities should implement this interface.
/// </summary>
/// <typeparam name="TId">Type of key of entity as known as primary key.</typeparam>
public interface IIdentifiable<TId>
{
    TId Id { get; }
}
