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

        protected async Task<ActionResult> SaveChanges(Object obj)
        {
            try
            {
                await _context.SaveChangesAsync();
                return Ok(obj);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "A concurrency error occurred while saving changes to the database.");
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while saving changes to the database.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}