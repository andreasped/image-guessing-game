using System;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.SharedKernel;
using Microsoft.EntityFrameworkCore;
using GuessingGameTest.Helpers;
using Xunit;
using Xunit.Abstractions;
using GuessingGame.Core.Domain.Game;
using System.Collections.Generic;

namespace GuessingGameTest.Domain.Game.Pipelines;

public class GetGameTests : DbTest
{
    public GetGameTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task GetGameByExistentUser_GetsCorrectGame()
    {
        using var context = new GameContext(ContextOptions, null);
        context.Database.Migrate();

        var userId = new Guid();
        var game = new GuessingGame.Core.Domain.Game.Game { GuesserId = userId, GameType = GameType.SinglePlayer, Image = new GameImage { ImgSource = "source", Label = "ImageLabel", Tiles = new List<GameTile>(), ImageId = 1 }, UsedTiles = new List<GameTile>() };
        
        var request = new GetGame.Request(userId);

        await context.Game.AddAsync(game);
        await context.SaveChangesAsync();

        var handler = new GetGame.Handler(context);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(game.GuesserId, result.GuesserId);
    }

    [Fact]
    public async Task GetGameByNonExistentUser_ReturnsNull()
    {
        {
            var nonExistentUserId = Guid.NewGuid();
            var request = new GetGame.Request(nonExistentUserId);

            using var context = new GameContext(ContextOptions, null);
            context.Database.Migrate();

            var handler = new GetGame.Handler(context);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.Null(result);
        }
    }
}
