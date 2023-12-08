using System.Collections.Generic;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game.Dto;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.Image;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using GuessingGameTest.Helpers;
using Xunit;
using Xunit.Abstractions;
using GuessingGame.Core.Domain.Game;

namespace GuessingGameTest.Domain.Game.Service
{
    public class EndGameTest : DbTest
    {
        public EndGameTest(ITestOutputHelper output) : base(output)
        {
        }
        [Fact]
        public async Task EndGame_ShouldRemoveGameFromDb()
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

            var userid = new System.Guid();
            var game = new GuessingGame.Core.Domain.Game.Game
            {
                GuesserId = userid,
                UsedTiles = new List<GameTile>(),
                Image = new GameImage { ImgSource = "source", Label = "ImageLabel", ImageId = 1 }
            };
            await context.Game.AddAsync(game);
            await context.SaveChangesAsync();

            await gameService.EndGame(game);


            var endedGame = await context.Game.FindAsync(game.Id);
            Assert.Null(endedGame);
        }
    }
}
