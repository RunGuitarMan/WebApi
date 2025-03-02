using Microsoft.AspNetCore.Mvc;
using WebApi.Application.Services;

namespace WebApi.Web.Controllers.Api;

[ApiController]
[Route("api/entity")]
public class PersistenceController(EntityService entityService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEntity(int id, CancellationToken cancellationToken)
    {
        var entity = await entityService.GetEntity(id, cancellationToken);
        if (entity == null)
        {
            return NotFound(id);
        }

        return Ok(entity);
    }
    
    [HttpPost]
    public async Task<IActionResult> PostEntity([FromBody] string content, CancellationToken cancellationToken)
    {
        var entity = await entityService.PostEntity(content, cancellationToken);
        return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
    }
}