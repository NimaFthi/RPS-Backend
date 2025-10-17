namespace RPS_Backend.WebSocket;

using System.Collections.Concurrent;
using System.Net.WebSockets;

public class WebSocketConnectionManager
{
    private ConcurrentDictionary<string, WebSocket> _connections = new();

    public void AddConnection(string connectionId, WebSocket socket)
    {
        _connections[connectionId] = socket;
    }

    public void RemoveConnection(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
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