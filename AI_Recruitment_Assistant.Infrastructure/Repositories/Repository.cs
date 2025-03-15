
using AI_Recruitment_Assistant.Domain.Repositories;
using AI_Recruitment_Assistant.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace AI_Recruitment_Assistant.Infrastructure.Repositories;

public class Repository(ApplicationDbContext context, ILogger<Repository> logger) : IRepository
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<Repository> _logger = logger;

    public async Task<bool> DeleteAsync<T>(T entity, CancellationToken cancellationToken) where T : class
    {
        try
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity");
            return false;
        }
    }

    public async Task<List<T>> GetAllAsync<T>(int? cursor, int? pageSize, CancellationToken cancellationToken) where T : class
    {
        try
        {
            var query = _context.Set<T>().AsQueryable();

            if (cursor is not null)
            {
                query = query.Where(e => EF.Property<int>(e, "Id") > cursor);
            }

            query = pageSize.HasValue ? query.Take((int)pageSize) : query;
            return await query.AsNoTracking().ToListAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    public async Task<T?> GetByIdAsync<T>(int id, CancellationToken cancellationToken) where T : class
    {
        try
        {
            return await _context.Set<T>()
              .AsNoTracking()
              .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);
        }
        catch (Exception)
        {

            throw;
        }

    }

    public async Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken) where T : class
    {
        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            _context.Set<T>().Attach(entity);
            entry.State = EntityState.Modified;
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency conflict occurred while updating entity of type {EntityType}", typeof(T).Name);

            foreach (var e in ex.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                if (databaseValues == null)
                {
                    _logger.LogError("The entity no longer exists in the database.");
                    throw new InvalidOperationException("The entity has been deleted by another transaction.");
                }

                // Update original values with database values
                entry.OriginalValues.SetValues(databaseValues);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity");
            throw;
        }
    }

    public async Task<List<T>> FindByConditionAsync<T>(Expression<Func<T, bool>> predicate, int? cursor = null, int? pageSize = null, CancellationToken cancellationToken = default) where T : class
    {
        var query = _context.Set<T>().Where(predicate).AsNoTracking();

        if (cursor is not null)
        {
            query = query.Where(e => EF.Property<int>(e, "Id") > cursor);
        }

        query = pageSize.HasValue ? query.Take((int)pageSize) : query;
        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<T> CreateAsync<T>(T entity, CancellationToken cancellationToken) where T : class
    {
        try
        {
            await _context.Set<T>().AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
