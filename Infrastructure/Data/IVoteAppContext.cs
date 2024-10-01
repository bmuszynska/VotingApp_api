using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public interface IVoteAppContext
    {
        DbSet<Voter> Voters { get; set; }
        DbSet<Candidate> Candidates { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}