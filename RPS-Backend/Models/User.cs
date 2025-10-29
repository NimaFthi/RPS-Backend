using RPS_Backend.Systems.User;

namespace RPS_Backend.Models;

public class User
{
    public int Id { get; set; }
    public Guid DeviceGuid { get; set; }
    public string UserId {get; set;}
    public string Username { get; set; }
    public int CoinsCount { get; set; }
    public int WinsCount { get; set; }
    public int LossesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }

    public UserProfile ToUserProfile()
    {
        return new UserProfile
        {
            UserId = UserId,
            Username = Username,
            CoinsCount = CoinsCount,
            WinsCount = WinsCount,
            LossesCount = LossesCount
        };
    }
}