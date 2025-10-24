namespace RPS_Backend.WebSocket;

using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;

public class WebSocketMiddleware : IMiddleware
{
    private readonly WebSocketServer _server;
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly JwtSettings _jwtSettings;

    public WebSocketMiddleware(WebSocketServer server
        ,WebSocketConnectionManager connectionManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _server = server;
        _connectionManager = connectionManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await next(context);
            return;
        }

        var token = context.Request.Query["token"].FirstOrDefault();
        if (string.IsNullOrEmpty(token) || !ValidateToken(token, out var deviceGuid))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        await _connectionManager.AddConnectionAsync(deviceGuid.ToString(), socket);

        // Delegate management to WebSocketServer
        await _server.HandleClientAsync(deviceGuid.ToString(), socket);

        await _connectionManager.RemoveConnectionAsync(deviceGuid.ToString(), "");
    }

    private bool ValidateToken(string token, out Guid deviceGuid)
    {
        deviceGuid = Guid.Empty;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // Retrieve DeviceGuid claim
            var guidClaim = principal.Claims.FirstOrDefault(c => c.Type == "deviceGuid")?.Value;
            if (guidClaim != null)
            {
                deviceGuid = Guid.Parse(guidClaim);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private async Task ReceiveMessages(string connectionId, WebSocket socket)
    {
        var buffer = new byte[1024 * 4];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server",
                    CancellationToken.None);
            }
            else
            {
                var receivedText = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"[WebSocket]: Message from {connectionId} → {receivedText}");

                // Echo test (send back)
                var echoBytes = Encoding.UTF8.GetBytes($"Echo: {receivedText}");
                await socket.SendAsync(new ArraySegment<byte>(echoBytes), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
        }
    }
}