using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class VotersController : BaseApiController
    {
        public VotersController(IVoteAppContext context) : base(context)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Voter>>> GetVoters()
        {
            try
            {
                return await _context.Voters.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Voter>> GetVoter(int id)
        {
            var voter = await _context.Voters.FindAsync(id);

            if (voter == null)
            {
                return NotFound();
            }

            return Ok(voter);
        }

        [HttpPost]
        public async Task<ActionResult<Voter>> AddNewVoter(Voter voter)
        {
            if (voter.Id != 0)
            {
                var existingVoter = await GetVoter(voter.Id);
                if (existingVoter.Result is not NotFoundResult)
                {
                    return Conflict("A voter with the same ID already exists.");
                }
            }

            _context.Voters.Add(voter);
            try
            {
                await _context.SaveChangesAsync();
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

            return CreatedAtAction(nameof(GetVoter), new { id = voter.Id }, voter);
        }

        [HttpPatch("{id}/hasVoted")]
        public async Task<ActionResult<Voter>> HasVoted(int id)
        {
            var voter = await _context.Voters.FindAsync(id);

            if (voter == null)
            {
                return NotFound();
            }

            voter.HasVoted = true;
            return SaveChanges(voter).Result;
        }
    }
}