namespace RPS_Backend.DB;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDBContext>
{
    // Hard-coded connection string here is JUST for design-time
    private const string CONNECTION_STRING = "Host=localhost;Port=5432;Database=RPS;Username=postgres;Password=123";
    
    public ApplicationDBContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDBContext>();

        optionsBuilder.UseNpgsql(CONNECTION_STRING);

        return new ApplicationDBContext(optionsBuilder.Options);
    }
}