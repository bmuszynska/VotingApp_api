using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected readonly IVoteAppContext _context;

        public BaseApiController(IVoteAppContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<T>> Get<T>(int id, DbSet<T> dbSet) where T : Person
        {
            var entity = await dbSet.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return Ok(entity);
        }

        public async Task<ActionResult<T>> Add<T>(T entity, DbSet<T> dbSet) where T : Person
        {
            if (entity.Id != 0)
            {
                var existingVoter = await Get<T>(entity.Id, dbSet);
                if (existingVoter.Result is not NotFoundResult)
                {
                    return Conflict("An entity with the same ID already exists.");
                }
            }

            dbSet.Add(entity);

            await _context.SaveChangesAsync();

            return Ok(entity);
        }
    }
}