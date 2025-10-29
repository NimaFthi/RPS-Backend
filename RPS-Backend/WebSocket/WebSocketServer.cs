using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using RPS_Backend.Systems;

namespace RPS_Backend.WebSocket;

public class WebSocketServer
{
    private readonly WebSocketConnectionManager _connectionManager;
    private readonly CommunicationService _communicationService;
    private readonly ILogger<WebSocketServer> _logger;

    private readonly ConcurrentDictionary<string, DateTime> _lastHeartbeat =
        new ();

    private static readonly TimeSpan HeartbeatTimeout = TimeSpan.FromSeconds(45);

    private static readonly TimeSpan HeartbeatCheckInterval = TimeSpan.FromSeconds(10);

    public WebSocketServer(WebSocketConnectionManager connectionManager, CommunicationService communicationService,ILogger<WebSocketServer> logger)
    {
        _connectionManager = connectionManager;
        _communicationService = communicationService;
        _logger = logger;

        _ = Task.Run(HeartbeatMonitorLoop);
    }

    public async Task HandleClientAsync(string connectionId, System.Net.WebSockets.WebSocket socket)
    {
        var buffer = new byte[1024 * 4];
        _logger.LogInformation($"Client connected: {connectionId}");

        _lastHeartbeat[connectionId] = DateTime.UtcNow;

        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _logger.LogInformation($"Client {connectionId} closed connection.");
                    await _connectionManager.RemoveConnectionAsync(connectionId, "Client closed connection.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await HandleMessageAsync(connectionId, message);
            }
        }
        catch (Exception ex)
        {
            await _connectionManager.RemoveConnectionAsync(connectionId, "Error in socket");
            _logger.LogError(ex, $"Error with connection {connectionId}");
        }
        finally
        {
            _lastHeartbeat.TryRemove(connectionId, out _);
            _logger.LogInformation($"Client {connectionId} disconnected.");
        }
    }

    private async Task HandleMessageAsync(string connectionId, string rawMessage)
    {
        _logger.LogInformation($"[Message from {connectionId}]: {rawMessage}");

        try
        {
            var payload = JsonConvert.DeserializeObject<WebSocketMessage>(rawMessage);
            if (payload == null) return;

            if (payload.Type == WebSocketMessageTypes.Ping)
            {
                _lastHeartbeat[connectionId] = DateTime.UtcNow;

                await SendMessageAsync(connectionId,
                    JsonConvert.SerializeObject(new WebSocketMessage
                    {
                        Type = WebSocketMessageTypes.Pong
                    }));
                
                return;
            }
            
            await _communicationService.ProcessRequest(this ,connectionId ,payload);
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
        if (socket == null || socket.State != WebSocketState.Open)
            return;

        var bytes = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(bytes),
                               WebSocketMessageType.Text,
                               true,
                               CancellationToken.None);
    }
    
    private async Task HeartbeatMonitorLoop()
    {
        while (true)
        {
            try
            {
                var now = DateTime.UtcNow;

                foreach (var kvp in _lastHeartbeat.ToArray())
                {
                    var connectionId = kvp.Key;
                    var lastPing = kvp.Value;
                    var elapsed = now - lastPing;

                    if (elapsed > HeartbeatTimeout)
                    {
                        _logger.LogWarning($"Client {connectionId} timed out after {elapsed.TotalSeconds:F0}s. Closing socket...");

                        _connectionManager.GetSocketById(connectionId, out var socket);
                        if (socket is {State: WebSocketState.Open})
                        {
                            try
                            {
                                _ = _connectionManager.RemoveConnectionAsync(connectionId, "Heartbeat timeout");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Error closing timed-out socket {connectionId}");
                            }
                        }

                        _lastHeartbeat.TryRemove(connectionId, out _);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in HeartbeatMonitorLoop");
            }

            await Task.Delay(HeartbeatCheckInterval);
        }
    }

    public class WebSocketMessage
    {
        public WebSocketMessageTypes Type { get; set; }
        public string? Data { get; set; }
    }
}
