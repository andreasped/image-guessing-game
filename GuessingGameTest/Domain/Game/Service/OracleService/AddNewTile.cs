using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.Core.Domain.Image;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using GuessingGameTest.Helpers;
using Xunit;
using Xunit.Abstractions;
using GuessingGame.Core.Domain.Game.Dto;
using System;

namespace GuessingGameTest.Domain.Game.Service;
public class AddNewTileTests : DbTest
{
    public AddNewTileTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task AddNewTileToGame_ShouldReturnANewTile()
    {

        var userid = new Guid();
        List<GameTile> gameTiles = new()
           {
                      new GameTile ("OneTile"),
            };

        var game = new GuessingGame.Core.Domain.Game.Game
        {
            GuesserId = userid,
            UsedTiles = new List<GameTile>(),
            Image = new GameImage { ImgSource = "source", Label = "ImageLabel", Tiles = gameTiles, ImageId = 1 }
        };

        var request = new AddNewTileToGame.Request(userid);

        using var context = new GameContext(ContextOptions, null);

        context.Database.Migrate();

        var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

        var mediatorMock = new Mock<IMediator>();

        var mockImageService = new Mock<IImageService>();
        mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1);
        mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

        var randomOracleService = new RandomOracleService(mockImageService.Object, context);

        var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

        await context.Game.AddAsync(game);
        await context.SaveChangesAsync();

        var handler = new AddNewTileToGame.Handler(context, randomOracleService);

        var result = await handler.Handle(request, CancellationToken.None);

        Assert.Single(game.UsedTiles);
    }

    [Fact]
    public async Task NonExistingGame_ShouldReturnNull()
    {
        var userid = new Guid();
        var request = new AddNewTileToGame.Request(userid);
        using var context = new GameContext(ContextOptions, null);

        context.Database.Migrate();

        var mediatorMock = new Mock<IMediator>();

        var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

        var mockImageService = new Mock<IImageService>();
        mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1);
        mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

        var randomOracleService = new RandomOracleService(mockImageService.Object, context);

        var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

        var handler = new AddNewTileToGame.Handler(context, randomOracleService);
        var exception = await Assert.ThrowsAsync<Exception>(async () => await handler.Handle(request, CancellationToken.None));

        Assert.Equal("Game not found", exception.Message);
    }
    [Fact]
    public async Task UsedTilesEqualsGameTiles_ShouldReturnNull()
    {
        List<GameTile> gameTiles = new();
        var image = new GameImage { ImgSource = "source", Label = "ImageLabel", Tiles = gameTiles, ImageId = 1 };
        var game = new GuessingGame.Core.Domain.Game.Game { Id = 1, Image = image, UsedTiles = gameTiles };
        using (var context = new GameContext(ContextOptions, null))
        {
            context.Database.Migrate();

            var mediatorMock = new Mock<IMediator>();

            var userid = new Guid();

            var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1);
            mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

            var randomOracleService = new RandomOracleService(mockImageService.Object, context);

            var gameService = new GameService(context, mediatorMock.Object, randomOracleService);


            await context.Game.AddAsync(game);
            await context.SaveChangesAsync();

            var request = new AddNewTileToGame.Request(userid);

            var handler = new AddNewTileToGame.Handler(context, randomOracleService);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.False(result);

        }
    }

    [Fact]
    public async Task SuccesfullyGettingNewTile_ShouldResetConsecutiveGuessCountToZero()
    {
        List<GameTile> gameTiles = new(
              new List<GameTile>
              {
                         new GameTile ("OneTile"),
              }
        );
        var image = new GameImage { ImgSource = "source", Label = "ImageLabel", Tiles = gameTiles, ImageId = 1 };
        var game = new GuessingGame.Core.Domain.Game.Game { Id = 1, Image = image };

        using (var context = new GameContext(ContextOptions, null))
        {
            context.Database.Migrate();

            await context.Game.AddAsync(game);
            await context.SaveChangesAsync();

            var userid = new Guid();

            var mediatorMock = new Mock<IMediator>();

            var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

            var mockImageService = new Mock<IImageService>();
            mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1);
            mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

            var randomOracleService = new RandomOracleService(mockImageService.Object, context);

            var gameService = new GameService(context, mediatorMock.Object, randomOracleService);


            var request = new AddNewTileToGame.Request(userid);

            var handler = new AddNewTileToGame.Handler(context, randomOracleService);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.Equal(0, game.ConsecutiveGuessCount);

        }
    }
}