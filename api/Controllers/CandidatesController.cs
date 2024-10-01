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
            return await _context.Candidates.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Candidate>> GetCandidate(int id)
        {
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            return candidate;
        }

        [HttpPost]
        public async Task<ActionResult<Candidate>> AddNewCandidate(Candidate candidate)
        {
            _context.Candidates.Add(candidate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCandidate), new { id = candidate.Id }, candidate);
        }

        [HttpPatch("{id}/gotVote")]
        public async Task<ActionResult<Candidate>> GotVote(int id)
        {
            var candidate = _context.Candidates.FindAsync(id).Result;
            if (candidate == null)
            {
                return NotFound();
            }

            candidate.VoteCount++;

            await _context.SaveChangesAsync();

            return Ok(candidate);
        }
    }
}