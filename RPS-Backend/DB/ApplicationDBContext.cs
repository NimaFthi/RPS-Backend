using Microsoft.EntityFrameworkCore;
using RPS_Backend.Models;

namespace RPS_Backend.DB;

public class ApplicationDBContext : DbContext
{
    public DbSet<User> Users { get; private set; }
    public DbSet<Match> MatchData { get; private set; }

    // Design-time constructor for migrations
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}