using RPS_Backend.DB;
using RPS_Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace RPS_Backend.Systems.User;


public class UserService
{
    private IServiceProvider _serviceProvider;
    private readonly GameSettings _gameSettings;
    private readonly ILogger<UserService> _logger;

    public UserService(IServiceProvider serviceProvider, IOptions<GameSettings> gameSettings, ILogger<UserService> logger)
    {
        _serviceProvider = serviceProvider;
        _gameSettings = gameSettings.Value;
        _logger = logger;
    }

    public async Task<Models.User?> GetUserByGuidAsync(Guid deviceGuid)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await db.Users.FirstOrDefaultAsync(u => u.DeviceGuid == deviceGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user with DeviceGuid: {DeviceGuid}", deviceGuid);
            return null;
        }
    }
    public async Task<UserProfile?> GetUserProfileByGuidAsync(Guid deviceGuid)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var user = await db.Users.FirstOrDefaultAsync(u => u.DeviceGuid == deviceGuid);
            return user?.ToUserProfile();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user with DeviceGuid: {DeviceGuid}", deviceGuid);
            return null;
        }
    }

    public async Task<bool> UserExistsAsync(Guid deviceGuid)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            return await db.Users.AnyAsync(u => u.DeviceGuid == deviceGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking existence of user with DeviceGuid: {DeviceGuid}", deviceGuid);
            return false;
        }
    }

    public async Task<bool> UpdateUserLogin(string userId)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            var result = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (result == null)
                return false;

            result.LastLogin = DateTime.UtcNow;
            await db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating LastLogin for user with UserId: {UserId}", userId);
            return false;
        }
    }

    public async Task<Models.User?> CreateNewUserAsync(Guid deviceGuid)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            
            var result = await db.Users.AddAsync(GetNewUser(deviceGuid));
            await db.SaveChangesAsync();
            return result.Entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new user for DeviceGuid: {DeviceGuid}", deviceGuid);
            return null;
        }
    }

    private Models.User GetNewUser(Guid deviceGuid)
    {
        var newUser = new Models.User
        {
            DeviceGuid = deviceGuid,
            CoinsCount = _gameSettings.NewUsersCoins,
            WinsCount = 0,
            LossesCount = 0,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };

        newUser.UserId = UserIdGenerator.GenerateUserId();
        newUser.Username = "Guest-" + newUser.UserId.Substring(0, 6);

        return newUser;
    }
}