using Newtonsoft.Json;
using RPS_Backend.Systems.User;
using RPS_Backend.WebSocket;

namespace RPS_Backend.Systems;

public class CommunicationService
{
    private UserService _userService;
    
    public CommunicationService(UserService userService)
    {
        _userService = userService;
    }

    public async Task ProcessRequest(WebSocketServer webSocketServer ,string connectionId, WebSocketServer.WebSocketMessage message)
    {
        switch (message.Type)
        {
            case WebSocketMessageType.GetInitData:
                break;
            case WebSocketMessageType.GetUserProfile:
                var userProfile = await _userService.GetUserProfileByGuidAsync(Guid.Parse((ReadOnlySpan<char>)connectionId));
                var json = JsonConvert.SerializeObject(userProfile);
                var jsonMessage = new WebSocketServer.WebSocketMessage
                {
                    RequestID = message.RequestID,
                    Type = WebSocketMessageType.GetUserProfile,
                    Data = json
                };
                await webSocketServer.SendMessageAsync(connectionId, JsonConvert.SerializeObject(jsonMessage));
                break;
            case WebSocketMessageType.RequestMatch:
                break;
            case WebSocketMessageType.CancelMatch:
                break;
            case WebSocketMessageType.MatchFound:
                break;
            case WebSocketMessageType.LeaveMatch:
                break;
            case WebSocketMessageType.StartMatch:
                break;
            case WebSocketMessageType.EndMatch:
                break;
            case WebSocketMessageType.StartRound:
                break;
            case WebSocketMessageType.RoundResult:
                break;
            case WebSocketMessageType.Move:
                break;
        }
    }
}