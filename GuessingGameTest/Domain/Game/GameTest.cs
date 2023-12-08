using Xunit;
using GuessingGame.Core.Domain.Game;
using System.Collections.Generic;
using System.Linq;

namespace GuessingGameTest.Domain.Game;

public class GameTests
{
    [Fact]
    public void AddNewTile_AddsOneTile()
    {

        var game = new GuessingGame.Core.Domain.Game.Game();
        var tile = new GameTile("GameTile");

        game.AddTile(tile);

        Assert.NotNull(game.UsedTiles);
        Assert.Single(game.UsedTiles);
        Assert.Equal(tile, game.UsedTiles[0]);
    }

    [Fact]
    public void AddGuess_IncrementsGuessCount()
    {
        var game = new GuessingGame.Core.Domain.Game.Game();

        game.AddGuess();

        Assert.Equal(1, game.GuessCount);
    }

    [Fact]
    public void CalculateScore_ShouldCalculateScoreCorrect()
    {
        var Game = new GuessingGame.Core.Domain.Game.Game
        {
            TimeUsedInPercent = 10,
            UsedTiles = new List<GameTile> { new("tile1"), new("tile2") },
            GuessCount = 3
        };

        Game.CalculateScore();

        Assert.Equal(1000 - 5 * Game.TimeUsedInPercent - 5 * (Game.UsedTiles.Count() - 1) - 5 * (Game.GuessCount - 1), Game.Score);
    }
}
