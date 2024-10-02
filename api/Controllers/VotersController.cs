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
            return await _context.Voters.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Voter>> GetVoter(int id)
        {
            return await Get<Voter>(id, _context.Voters);
        }

        [HttpPost]
        public async Task<ActionResult<Voter>> AddNewVoter(Voter voter)
        {
            voter.HasVoted = false;
            return await Add<Voter>(voter, _context.Voters);
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
            await _context.SaveChangesAsync();

            return Ok(voter);
        }
    }
}