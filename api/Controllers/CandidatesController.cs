using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    public class CandidatesController : BaseApiController
    {
        private readonly VoteAppContext _context;

        public CandidatesController(VoteAppContext context)
        {
            _context = context;
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

        [HttpPut("{id}")]
        public async Task<ActionResult<Candidate>> CandidateGotVote(int id)
        {
            var candidate = _context.Candidates.FindAsync(id).Result;
            if (candidate == null)
            {
                return BadRequest();
            }

            candidate.VoteCount++;

            await _context.SaveChangesAsync();

            return Ok(candidate);
        }

        /*
                [HttpPut("{id}")]
                public async Task<ActionResult<Candidate>> CandidateGotVote(int id, Candidate candidate)
                {
                    if (id != candidate.Id)
                    {
                        return BadRequest();
                    }

                    var candidateOriginal = _context.Candidates.FindAsync(id);
                    candidateOriginal.Result.VoteCount++;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }

                    return Ok(candidate);
                }*/
    }
}