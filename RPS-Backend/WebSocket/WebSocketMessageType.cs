namespace RPS_Backend.WebSocket;

public enum WebSocketMessageType
{
    Ping = 1,
    Pong = 2,
    GetInitData = 3,
    GetUserProfile = 4,
    RequestMatch = 100,
    CancelMatch = 101,
    MatchFound = 102,
    LeaveMatch = 103,
    StartMatch = 104,
    EndMatch = 105,
    StartRound = 106,
    RoundResult = 107,
    Move = 108
}