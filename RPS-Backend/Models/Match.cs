using RPS_Backend.Systems.Matchmaking;

namespace RPS_Backend.Models;

public class Match
{
    public int Id { get; set; }
    public int MatchId {get; set;}
    public string MatchName { get; set; }
    public int Entry { get; set; }
    public int Prize { get; set; }

    public MatchData GetMatchData()
    {
        return new MatchData()
        {
            MatchId = MatchId,
            MatchName = MatchName,
            Entry = Entry,
            Prize = Prize,
        };
    }
}