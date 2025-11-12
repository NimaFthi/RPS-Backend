using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPS_Backend.DB;
using RPS_Backend.Models;
using RPS_Backend.Systems;
using RPS_Backend.Systems.Authentication;
using RPS_Backend.Systems.GetInitData;
using RPS_Backend.Systems.User;
using RPS_Backend.WebSocket;

namespace RPS_Backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddSingleton<GetInitDataService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<CommunicationService>();
        builder.Services.AddSingleton<WebSocketConnectionManager>();
        builder.Services.AddSingleton<WebSocketServer>();
        builder.Services.AddSingleton<JwtTokenGenerator>();
        builder.Services.AddTransient<WebSocketMiddleware>();
        
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
        builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings")); //TODO : Move this into data base instead of app settings

        builder.Services.AddDbContext<ApplicationDBContext>(options => options.UseNpgsql(builder.Configuration.GetSection("ConnectionStrings")["Postgres"]));
        
        var app = builder.Build();

        app.UseWebSockets();
        app.UseMiddleware<WebSocketMiddleware>();
        
        app.MapControllers();

        app.Run();
    }
}