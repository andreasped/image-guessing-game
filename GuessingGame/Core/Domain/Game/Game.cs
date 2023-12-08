using System.Data;
using GuessingGame.SharedKernel;

namespace GuessingGame.Core.Domain.Game;

public class Game : BaseEntity
{

    public Game()
    {
        UsedTiles = new List<GameTile>();
    }

    public Game(Guid guesserId, Guid proposerId, GameType gameType, GameStatus gameStatus, GameImage image)
    {
        GuesserId = guesserId;
        ProposerId = proposerId;
        GameType = gameType;
        GameStatus = gameStatus;
        Image = image;
    }

    public int Id { get; set; }
    public Guid GuesserId { get; set; }
    public Guid ProposerId { get; set; }
    public int ConsecutiveGuessCount { get; set; }
    public int GuessCount { get; set; }
    public int Score { get; set; }
    public DateTime dateTime { get; set; } = DateTime.Now;
    public GameType GameType { get; set; }
    public GameStatus GameStatus { set; get; }
    public GameImage Image { get; set; }
    public List<GameTile> UsedTiles { get; set; } = new List<GameTile>();
    public int TimeUsedInPercent { get; set; }
    public TimeSpan TimeUsed { get; set; }
    private int maxGuesses = 3;

    public void CalculateScore()
    {
        var STARTING_VALUE = 1000;
        Score = STARTING_VALUE - 5 * TimeUsedInPercent - 5 * (UsedTiles.Count() - 1) - 5 * (GuessCount - 1);
        if (Score < 0)
        {
            Score = 0;
        }
    }

    public List<GameTile> GetUnusedTiles()
    {
        return Image.Tiles.Except(UsedTiles).ToList();
    }

    public void AddTile(GameTile tile)
    {
        if (UsedTiles == null)
        {
            UsedTiles = new List<GameTile>();
        }
        ResetConsecutiveGuessCount();
        UsedTiles.Add(tile);
    }

    public void ResetConsecutiveGuessCount()
    {
        ConsecutiveGuessCount = 0;
    }

    public void AddGuess()
    {
        GuessCount++;
        ConsecutiveGuessCount++;
    }

    public bool HasUsedAllTiles()
    {
        return UsedTiles.Count() == Image.Tiles.Count();
    }

    public bool HasReachedMaxGuesses()
    {
        return ConsecutiveGuessCount == maxGuesses;
    }
}