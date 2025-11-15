using Newtonsoft.Json;
using RPS_Backend.Systems.GetInitData;
using RPS_Backend.Systems.User;
using RPS_Backend.WebSocket;

namespace RPS_Backend.Systems;

public class CommunicationService
{
    private GetInitDataService _getInitDataService;
    private UserService _userService;
    
    public CommunicationService(GetInitDataService getInitDataService,UserService userService)
    {
        _getInitDataService = getInitDataService;
        _userService = userService;
    }

    public async Task ProcessRequest(WebSocketServer webSocketServer ,string connectionId, WebSocketServer.WebSocketMessage message)
    {
        string json = "";
        
        switch (message.Type)
        {
            case WebSocketMessageType.GetInitData:
                var initData = await _getInitDataService.GetInitData();
                json = JsonConvert.SerializeObject(initData);
                break;
            case WebSocketMessageType.GetUserProfile:
                var userProfile = await _userService.GetUserProfileByGuidAsync(Guid.Parse((ReadOnlySpan<char>)connectionId));
                json = JsonConvert.SerializeObject(userProfile);
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
        
        if(string.IsNullOrEmpty(json)) return;
        
        var respondMessage = new WebSocketServer.WebSocketMessage
        {
            RequestID = message.RequestID,
            Type = message.Type,
            Data = json
        };
        await webSocketServer.SendMessageAsync(connectionId, JsonConvert.SerializeObject(respondMessage));
    }
}