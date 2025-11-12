using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPS_Backend.DB;
using RPS_Backend.Models;
using RPS_Backend.Systems.Matchmaking;

namespace RPS_Backend.Systems.GetInitData;

public class GetInitDataService
{
    private IServiceProvider _serviceProvider;
    private GameSettings _gameSettings;
    private ILogger<GetInitDataService> _logger;
    
    public GetInitDataService(IServiceProvider serviceProvider,  IOptions<GameSettings> gameSettings, ILogger<GetInitDataService> logger)
    {
        _serviceProvider = serviceProvider;
        _gameSettings = gameSettings.Value;
        _logger = logger;
    }

    public async Task<InitData> GetInitData()
    {
        var matchData = await GetAvailableMatch();
        return new InitData()
        {
            AvailableMatchData = matchData
        };
    }
    
    private async Task<List<MatchData>> GetAvailableMatch()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            var data = await db.MatchData.ToListAsync();
            return data.Select(x => x.GetMatchData()).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching match data");
            return new();
        }
    }
}