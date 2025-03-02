using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using WebApi.Domain;
using WebApi.Persistence;
using ProtoBuf;
using WebApi.Application.Models;

namespace WebApi.Application.Services;

public class EntityService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDistributedCache _cache;
    private const int CacheTtlSeconds = 600;
    
    public EntityService(ApplicationDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<TestEntity?> GetEntity(int id, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"TestEntity_{id}";

        var cachedData = await _cache.GetAsync(cacheKey, cancellationToken);
        if (cachedData != null)
        {
            using var memoryStream = new MemoryStream(cachedData);
            var dto = Serializer.Deserialize<TestEntityProtoDto>(memoryStream);
            return dto.ToDomainModel();
        }

        var entity = await _dbContext.TestEntities
            .AsNoTracking()
            .SingleOrDefaultAsync(testEntity => testEntity.Id == id, cancellationToken);

        if (entity == null)
        {
            return null;
        }

        var entityDto = new TestEntityProtoDto(entity);
        byte[] serializedData;
        using (var memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, entityDto);
            serializedData = memoryStream.ToArray();
        }

        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheTtlSeconds)
        };

        await _cache.SetAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        return entity;
    }
    
    public async Task<TestEntity> PostEntity(string content, CancellationToken cancellationToken)
    {
        var entity = new TestEntity
        {
            Content = content
        };
        
        await _dbContext.TestEntities.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        string cacheKey = $"TestEntity_{entity.Id}";
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheTtlSeconds)
        };
        
        var entityDto = new TestEntityProtoDto(entity);
        
        byte[] serializedData;
        using (var memoryStream = new MemoryStream())
        {
            Serializer.Serialize(memoryStream, entityDto);
            serializedData = memoryStream.ToArray();
        }
        
        await _cache.SetAsync(cacheKey, serializedData, cacheOptions, cancellationToken);

        return entity;
    }
}