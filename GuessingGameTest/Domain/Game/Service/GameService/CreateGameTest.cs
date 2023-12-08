using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Dto;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.Image;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using GuessingGameTest.Helpers;
using Xunit;
using Xunit.Abstractions;
using Moq;


namespace GuessingGameTest.Domain.Game.Service
{
    public class CreateGameTests : DbTest
    {
        public CreateGameTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task CreateSinglePLayerGameWithValidInput_ShouldSucceed()
        {
            using var context = new GameContext(ContextOptions, null);
            context.Database.Migrate();
            var mediatorMock = new Mock<IMediator>();

            var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1);
            mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

            var randomOracleService = new RandomOracleService(mockImageService.Object, context);

            var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

            var userid = Guid.NewGuid();

            var gameType = GameType.SinglePlayer;

            await gameService.CreateGame(userid, gameType);

            var createdGame = await context.Game.FirstOrDefaultAsync(g => g.GuesserId == userid);

            Assert.NotNull(createdGame);
            Assert.Equal(userid, createdGame.GuesserId);
            Assert.Equal(gameType, createdGame.GameType);
        }

            [Fact]
            public async Task CreateTwoPLayerGameWithValidInput_ShouldSucceed()
            {
            using var context = new GameContext(ContextOptions, null);
            context.Database.Migrate();
            var mediatorMock = new Mock<IMediator>();

            var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(5);
            mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

            var randomOracleService = new RandomOracleService(mockImageService.Object, context);

            var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

            var userid = Guid.NewGuid();

            var gameType = GameType.TwoPlayer;

            await gameService.CreateGame(userid, gameType);

            var createdGame = await context.Game.FirstOrDefaultAsync(g => g.ProposerId == userid);
            
            Assert.NotNull(createdGame);
            Assert.Equal(userid, createdGame.ProposerId);
            Assert.Equal(gameType, createdGame.GameType);
        }
    }
}