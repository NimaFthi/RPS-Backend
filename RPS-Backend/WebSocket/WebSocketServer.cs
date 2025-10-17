using Newtonsoft.Json;

namespace RPS_Backend.WebSocket;
using System.Net.WebSockets;
using System.Text;

public class WebSocketServer
{
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly ILogger<WebSocketServer> _logger;

    public WebSocketServer(WebSocketConnectionManager connectionManager, ILogger<WebSocketServer> logger)
    {
        _connectionManager = connectionManager;
        _logger = logger;
    }

    public async Task HandleClientAsync(string connectionId, WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        _logger.LogInformation($"Client connected: {connectionId}");

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation($"Client {connectionId} closed connection.");
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await HandleMessageAsync(connectionId, message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error with connection {connectionId}");
        }
        finally
        {
            _logger.LogInformation($"Client {connectionId} disconnected.");
        }
    }

    private async Task HandleMessageAsync(string connectionId, string rawMessage)
    {
        _logger.LogInformation($"[Message from {connectionId}]: {rawMessage}");

        try
        {
            var payload = JsonConvert.DeserializeObject<WebSocketMessage>(rawMessage);

            switch (payload.Type)
            {
                case WebSocketMessageTypes.Ping:
                    await SendMessageAsync(connectionId, JsonConvert.SerializeObject(
                        new WebSocketMessage(WebSocketMessageTypes.Pong,
                            JsonConvert.SerializeObject(DateTime.UtcNow))));
                    break;
                case WebSocketMessageTypes.GetInitData:
                    break;
                case WebSocketMessageTypes.GetUserProfile:
                    break;
                case WebSocketMessageTypes.RequestMatch:
                    break;
                case WebSocketMessageTypes.CancelMatch:
                    break;
                case WebSocketMessageTypes.MatchFound:
                    break;
                case WebSocketMessageTypes.LeaveMatch:
                    break;
                case WebSocketMessageTypes.StartMatch:
                    break;
                case WebSocketMessageTypes.EndMatch:
                    break;
                case WebSocketMessageTypes.StartRound:
                    break;
                case WebSocketMessageTypes.RoundResult:
                    break;
                case WebSocketMessageTypes.Move:
                    break;
                default:
                    _logger.LogWarning($"Unknown message type from {connectionId}: {payload.Type}");
                    break;
            }
        }
        catch (JsonException je)
        {
            _logger.LogWarning(je, $"Invalid JSON from {connectionId}: {rawMessage}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in message handler for {connectionId}");
        }
    }
    
    public async Task SendMessageAsync(string connectionId, string message)
    {
        _connectionManager.GetSocketById(connectionId, out var socket);
        
        if(socket == null)
            return;
        
        if (socket.State != WebSocketState.Open)
            return;
        
        var bytes = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }

    public struct WebSocketMessage(
        WebSocketMessageTypes type,
        string data)
    {
        public WebSocketMessageTypes Type => type;
        public string Data => data;
    }
}