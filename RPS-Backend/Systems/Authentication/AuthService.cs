using RPS_Backend.Systems.User;

namespace RPS_Backend.Systems.Authentication;

public class AuthService
{
    private UserService _userService;
    private JwtTokenGenerator _tokenGenerator;
    private ILogger<AuthService> _logger; 

    public AuthService(UserService userService, JwtTokenGenerator jwtTokenGenerator, ILogger<AuthService> logger)
    {
        _userService = userService;
        _tokenGenerator = jwtTokenGenerator;
        _logger = logger;
    }

    public string GenerateToken(Guid deviceGuid, string username = null)
    {
        return _tokenGenerator.GenerateToken(deviceGuid, username);
    }

    public async Task<Models.User?> Login(Guid deviceGuid)
    {
        try
        {
            if (!await _userService.UserExistsAsync(deviceGuid))
            {
                return await _userService.CreateNewUserAsync(deviceGuid);
            }

            return await _userService.GetUserByGuidAsync(deviceGuid);
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in logging in => {e.Message} || {e.StackTrace}");
            return null;
        }
    }
}