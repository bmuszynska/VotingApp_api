using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class CandidatesController : BaseApiController
    {
        // private readonly IVoteAppContext _context;

        public CandidatesController(IVoteAppContext context) : base(context)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Candidate>>> GetCandidates()
        {
            try
            {
                return await _context.Candidates.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Candidate>> GetCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            return Ok(candidate);
        }

        [HttpPost]
        public async Task<ActionResult<Candidate>> AddNewCandidate(Candidate candidate)
        {
            if (candidate.Id != 0)
            {
                var existingVoter = await GetCandidate(candidate.Id);
                if (existingVoter.Result is not NotFoundResult)
                {
                    return Conflict("A voter with the same ID already exists.");
                }
            }

            _context.Candidates.Add(candidate);
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

            return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate); ;
        }

        [HttpPatch("{id}/gotVote")]
        public async Task<ActionResult<Candidate>> GotVote(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);

            if (candidate == null)
            {
                return NotFound();
            }

            candidate.VoteCount++;

            return SaveChanges(candidate).Result;
        }
    }
}