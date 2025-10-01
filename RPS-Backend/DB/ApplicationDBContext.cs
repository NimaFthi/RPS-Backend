using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPS_Backend.Models;

namespace RPS_Backend.DB;

public class ApplicationDBContext : DbContext
{
    private readonly string _connectionString;
    public DbSet<User> Users { get; private set; }

    public ApplicationDBContext(IOptions<ConnectionStrings> connections)
    {
        _connectionString = connections.Value.Postgres;
    }

    // Design-time constructor for migrations
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && !string.IsNullOrEmpty(_connectionString))
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}