using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class VoteAppContext : DbContext
    {
        public VoteAppContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Voter> Voters { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
    }
}