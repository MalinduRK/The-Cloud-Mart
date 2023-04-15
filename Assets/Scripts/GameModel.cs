using Realms;

public class GameModel : RealmObject
{

    [PrimaryKey]
    public string gamerTag { get; set; }

    public int redScore { get; set; }
    public int greenScore { get; set; }
    public int whiteScore { get; set; }

    public GameModel() { }

    public GameModel(string gamerTag, int redScore, int greenScore, int whiteScore)
    {
        this.gamerTag = gamerTag;
        this.redScore = redScore;
        this.greenScore = greenScore;
        this.whiteScore = whiteScore;
    }

}