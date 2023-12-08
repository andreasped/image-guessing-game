using System;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Result.Events;
using GuessingGame.Core.Domain.Result.Handlers;
using GuessingGame.Core.Domain.Results.Dto;
using GuessingGame.SharedKernel;
using Microsoft.EntityFrameworkCore;
using GuessingGameTest.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace GuessingGameTest.Domain.Result.Handlers;

    public class EndedGameHandlerTests : DbTest
    {
        public EndedGameHandlerTests(ITestOutputHelper output) : base(output)
        {
        }   

        [Fact]
        public async Task ValidResults_ShouldAddResultsToDatabase(){
        using var context = new GameContext(ContextOptions, null);
        context.Database.Migrate();

        var handler = new GameEndedHandler(context);

        var endedGame = new EndedGame(new GameResultDto
        (
            0,
            Guid.NewGuid(),
            Guid.NewGuid(),
            100,
            DateTime.Now,
            GameType.SinglePlayer,
            3,
            TimeSpan.FromSeconds(10),
            10
        ));
        await handler.Handle(endedGame, CancellationToken.None);

        var leaderboard = await context.Leaderboards.SingleOrDefaultAsync();

        Assert.NotNull(leaderboard);
        Assert.Equal(endedGame.GameResultDto.GuesserId, leaderboard.GuesserId);
        Assert.Equal(endedGame.GameResultDto.ProposerId, leaderboard.ProposerId);
        Assert.Equal(endedGame.GameResultDto.Score, leaderboard.Score);
        Assert.Equal(endedGame.GameResultDto.GameType == GameType.TwoPlayer ? ResultType.TwoPlayer : ResultType.SinglePlayer, leaderboard.ResultType);
        Assert.Equal(endedGame.GameResultDto.dateTime, leaderboard.DateTime);
        Assert.Equal(endedGame.GameResultDto.ImageId, leaderboard.ImageId);
        Assert.Equal(endedGame.GameResultDto.TimeUsed, leaderboard.TimeUsed);
        Assert.Equal(endedGame.GameResultDto.GuessCount, leaderboard.GuessCount);
    }
    }

