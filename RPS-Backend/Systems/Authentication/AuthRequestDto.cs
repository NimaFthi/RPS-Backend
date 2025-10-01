namespace RPS_Backend.Systems.Authentication;

[Serializable]
public class AuthRequestDto
{
    public Guid DeviceGUID { get; set; }
}