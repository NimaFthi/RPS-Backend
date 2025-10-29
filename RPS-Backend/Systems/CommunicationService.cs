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
            case WebSocketMessageTypes.GetInitData:
                break;
            case WebSocketMessageTypes.GetUserProfile:
                var userProfile = await _userService.GetUserProfileByGuidAsync(Guid.Parse((ReadOnlySpan<char>)connectionId));
                var json = JsonConvert.SerializeObject(userProfile);
                var jsonMessage = new WebSocketServer.WebSocketMessage
                {
                    Type = WebSocketMessageTypes.GetUserProfile,
                    Data = json
                };
                await webSocketServer.SendMessageAsync(connectionId, JsonConvert.SerializeObject(jsonMessage));
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
        }
    }
}