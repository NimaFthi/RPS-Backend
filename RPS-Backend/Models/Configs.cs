namespace RPS_Backend.Models;

public class GameSettings
{
    public int NewUsersCoins {get; set;}
    public int MaxRounds {get; set;}
    public int MatchmakingTimeoutSeconds  {get; set;}
}

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public int ExpiryMinutes { get; set; }
}

public class ConnectionStrings
{
    public string Postgres { get; set; }
}