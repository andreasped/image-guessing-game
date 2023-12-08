namespace GuessingGame.Core.Domain.Results;

public class Leaderboard
{
    public Leaderboard() { }

    public Leaderboard(int gameId, Guid guesserId, Guid proposerId, int score, ResultType resultType, DateTime dateTime, int imageId, TimeSpan timeUsed, int guessCount)
    {
        GameId = gameId;
        GuesserId = guesserId;
        ProposerId = proposerId;
        Score = score;
        ResultType = resultType;
        this.DateTime = dateTime;
        ImageId = imageId;
        TimeUsed = timeUsed;
        GuessCount = guessCount;
    }

    public int Id { get; protected set; }
    public int GameId { get; set; }
    public Guid GuesserId { get; set; }
    public Guid ProposerId { get; set; }
    public int Score { get; set; }
    public ResultType ResultType { get; set; }
    public DateTime DateTime { get; set; }
    public int ImageId { get; set; } // Saving image from a game for scalability purposes
    public TimeSpan TimeUsed { get; set; }
    public int GuessCount { get; set; }
}