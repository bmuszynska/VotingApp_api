using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class CandidatesController : BaseApiController
    {
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
            return await Get<Candidate>(id, _context.Candidates);
        }

        [HttpPost]
        public async Task<ActionResult<Candidate>> AddNewCandidate(Candidate candidate)
        {
            candidate.VoteCount = 0;
            return await Add<Candidate>(candidate, _context.Candidates);
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
            await _context.SaveChangesAsync();

            return Ok(candidate);
        }
    }
}