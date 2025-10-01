using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RPS_Backend.Systems.Authentication;

namespace RPS_Backend.Controllers;

[Controller]
[Route("api/auth")]
public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;
    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] AuthRequestDto requestDto)
    {
        try
        {
            var user = await _authService.Login(requestDto.DeviceGUID);
            if (user == null)
            {
                return new NotFoundObjectResult("User not found");
            }
            
            return new OkObjectResult(_authService.GenerateToken(user.DeviceGuid, user.Username));
        }
        catch (Exception e)
        {
            _logger.LogError($"Error in logging in => {e.Message} || {e.StackTrace}");
            return new StatusCodeResult(500);
        }
    }
}