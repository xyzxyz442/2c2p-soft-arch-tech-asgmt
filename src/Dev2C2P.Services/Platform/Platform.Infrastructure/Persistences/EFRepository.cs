using Dev2C2P.Services.Platform.Application.Abstractions;
using Dev2C2P.Services.Platform.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Dev2C2P.Services.Platform.Infrastructure.Persistences;

/// <summary>
///
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <typeparam name="TBase"></typeparam>
/// <typeparam name="TId"></typeparam>
/// <typeparam name="TUniqueId"></typeparam>
public abstract class EFRepository<TDbContext, TBase, TId, TUniqueId> : IEFRepository<TBase, TId>
    where TDbContext : EFDbContext<TBase, TId>
    where TBase : class, IIdentifiable<TId>
    where TUniqueId : class
{
    protected readonly ILogger<EFRepository<TDbContext, TBase, TId, TUniqueId>> Logger;

    protected readonly TDbContext Context;

    protected EFRepository(TDbContext context, ILogger<EFRepository<TDbContext, TBase, TId, TUniqueId>> logger)
    {
        Context = context;
        Logger = logger;
    }

    public virtual async Task<IEnumerable<T>> GetAsync<T>(
        Expression<Func<T, bool>>? filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
        int? skip,
        int? take)
        where T : class, TBase
    {
        IQueryable<T> query = Context.Set<T>();

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<bool> UpdateOneAsync<T>(T entity)
        where T : class, TBase
    {
        try
        {
            var existingEntity = await Context.Set<T>().FindAsync(entity.Id);
            if (existingEntity != null)
            {
                Context.Entry(existingEntity).CurrentValues.SetValues(entity);
            }
            else
            {
                await Context.Set<T>().AddAsync(entity);
            }

            await Context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error occurred while updating entity.");
            return false;
        }
    }

    protected virtual IQueryable<T> GetQueryable<T>(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null)
            where T : class, TBase
    {
        IQueryable<T> query = DoGetQueryable<T>();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (skip.HasValue)
        {
            query = query.Skip(skip.Value);
        }

        if (take.HasValue)
        {
            query = query.Take(take.Value);
        }

        return query;
    }

    /// <summary>
    /// Get instance of an queryable instance of <see cref="DbSet{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of entity that IS-A relation to <see cref="TBase"/> which implement <see cref="IEntity{TId}"/>.</typeparam>
    /// <returns>An instance of an queryable instance of <see cref="DbSet{T}"/>.</returns>
    protected virtual IQueryable<T> DoGetQueryable<T>()
        where T : class, TBase
    {
        return GetDbSet<T>();
    }

    /// <summary>
    /// Get instance of an <see cref="DbSet{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of entity that IS-A relation to <see cref="TBase"/> which implement <see cref="IEntity{TId}"/>.</typeparam>
    /// <returns>An <see cref="DbSet{T}"/>.</returns>
    protected virtual DbSet<T> GetDbSet<T>()
        where T : class, TBase
    {
        return Context.Set<T>();
    }
}
