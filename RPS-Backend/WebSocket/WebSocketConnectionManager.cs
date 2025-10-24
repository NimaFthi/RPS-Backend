namespace RPS_Backend.WebSocket;

using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketConnectionManager
{
    private ConcurrentDictionary<string, WebSocket> _connections = new();

    public async Task AddConnectionAsync(string connectionId, WebSocket socket)
    {
        await RemoveConnectionAsync(connectionId, "New socket");
        _connections[connectionId] = socket;
    }

    public async Task RemoveConnectionAsync(string connectionId, string reason)
    {
        if (_connections.TryRemove(connectionId, out var socket))
        {
            if (socket is {State: WebSocketState.Open or WebSocketState.CloseReceived})
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
                }
                catch{}
            }
            
            socket.Dispose();
        }
    }

    public void GetSocketById(string connectionId, out WebSocket? connectionSocket)
    {
        _connections.TryGetValue(connectionId, out var socket);
        connectionSocket = socket;
    }

    public ConcurrentDictionary<string, WebSocket> GetAllConnections()
    {
        return _connections;
    }
}