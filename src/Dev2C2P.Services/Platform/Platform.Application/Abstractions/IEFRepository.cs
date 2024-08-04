using Dev2C2P.Services.Platform.Domain.Abstractions;

namespace Dev2C2P.Services.Platform.Application.Abstractions;

public interface IEFRepository<TBase, TId, TUniqueId>
    where TBase : class, IIdentifiable<TId>
{
    /// <summary>
    /// Asynchronously, get entities.
    /// </summary>
    /// <typeparam name="T">Type of entity that IS-A relation to <see cref="TBase"/> which implement <see cref="IIdentifiable{TId}"/>.</typeparam>
    /// <param name="filter">A function to filter the result.</param>
    /// <param name="orderBy">A function to sort the result.</param>
    /// <param name="skip">Specific number to skip.</param>
    /// <param name="take">Specific number to take.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IEnumerable{T}"/> of entities.</returns>
    Task<IEnumerable<T>> GetAsync<T>(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null)
        where T : class, TBase;

    /// <summary>
    /// Asynchronously, get entity by unique id.
    /// </summary>
    /// <typeparam name="T">Type of entity that IS-A relation to <see cref="TBase"/> which implement <see cref="IEntity{TId}"/>.</typeparam>
    /// <param name="uniqueId">Specific a unique id.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an entity that match given unique key or null.</returns>
    Task<T?> GetByUniqueIdAsync<T>(TUniqueId uniqueId)
        where T : class, TBase;

    /// <summary>
    /// Asynchronously, update entity.
    /// </summary>
    /// <typeparam name="T">Type of entity that IS-A relation to <see cref="TBase"/> which implement <see cref="IIdentifiable{TId}"/>.</typeparam>
    /// <param name="entity">The entity to update</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <see langword="true"/> if success, otherwise is <see langword="false"/>.</returns>
    Task<bool> UpdateOneAsync<T>(T entity)
        where T : class, TBase;
}
