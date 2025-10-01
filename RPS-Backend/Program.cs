using Microsoft.Extensions.Options;
using RPS_Backend.DB;
using RPS_Backend.Models;
using RPS_Backend.Systems.Authentication;
using RPS_Backend.Systems.User;

namespace RPS_Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<JwtTokenGenerator>();
        builder.Services.AddSingleton<AuthService>();
        
        builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
        builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings")); //TODO : Move this into data base instead of app settings
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

        builder.Services.AddDbContext<ApplicationDBContext>();
        
        var app = builder.Build();
        
        app.MapControllers();

        app.Run();
    }
}