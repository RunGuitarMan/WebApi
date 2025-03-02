using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Domain;
using WebApi.Persistence;

namespace WebApi.Application.Services;

public class EntityService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public EntityService(ApplicationDbContext dbContext, IMemoryCache memoryCache)
    {
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task<TestEntity?> GetEntity(int id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"TestEntity_{id}";
        
        bool exists = _memoryCache.TryGetValue(cacheKey, out TestEntity? cachedEntity);
        if (exists && cachedEntity != null)
        {
            return cachedEntity;
        }

        var entity = await _dbContext.TestEntities
            .AsNoTracking()
            .SingleOrDefaultAsync(testEntity => testEntity.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        _memoryCache.Set(cacheKey, entity, TimeSpan.FromSeconds(300));
        
        return entity;
    }
    
    public async Task<TestEntity> PostEntity(string content, CancellationToken cancellationToken)
    {
        var entity = new TestEntity
        {
            Content = content
        };
        
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        await _dbContext.TestEntities.AddAsync(entity, cancellationToken);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        
        return entity;
    }
}