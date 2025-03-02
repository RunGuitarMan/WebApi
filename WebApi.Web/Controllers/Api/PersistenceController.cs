using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Domain;
using WebApi.Persistence;

namespace WebApi.Web.Controllers.Api;

[ApiController]
[Route("api/persistence")]
public class PersistenceController(ApplicationDbContext dbContext, IMemoryCache cache) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntity(int id, CancellationToken cancellationToken)
    {
        string cacheKey = $"TestEntity_{id}";

        if (cache.TryGetValue(cacheKey, out TestEntity? cachedEntity))
        {
            return Ok(cachedEntity);
        }

        var entity = await dbContext.TestEntities
            .AsNoTracking()
            .SingleOrDefaultAsync(testEntity => testEntity.Id == id, cancellationToken);

        if (entity == null)
        {
            return NotFound(id);
        }

        cache.Set(cacheKey, entity, TimeSpan.FromSeconds(300));

        return Ok(entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> PostEntity(string content, CancellationToken cancellationToken)
    {
        var entity = new TestEntity
        {
            Content = content
        };
   
        await dbContext.TestEntities.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
    }
}