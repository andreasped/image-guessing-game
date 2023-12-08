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

namespace GuessingGameTest.Domain.Game.Service;
public class VerifyGuessTests : DbTest
{
    public VerifyGuessTests(ITestOutputHelper output) : base(output)
    {
    }

    [Theory]
    [InlineData("ImageLabel")]
    [InlineData("imagelabel")]
    [InlineData(" ImageLabel")]
    [InlineData("ImageLabel ")]
    public async Task VerifyCorrectGuess_ShouldReturnTrueTrue(string guess)
    {
        var image = new GuessingGame.Core.Domain.Image.Image("source", new List<Tile>(), "ImageLabel");

        var game = new GuessingGame.Core.Domain.Game.Game { Image = new GameImage { ImgSource = "source", Label = "ImageLabel", Tiles = new List<GameTile>(), ImageId = 1 } };
        using var context = new GameContext(ContextOptions, null);
        context.Database.Migrate();

        await context.Game.AddAsync(game);
        await context.SaveChangesAsync();

        List<Tile> gameTiles = new()
        {
                    new Tile ("FirstTile"),
                    new Tile ("SecondTile"),
                    new Tile ("ThirdTile"),
                    new Tile ("FourthTile")
                };

        var imagedto = new ImageDto(1, "Src", "ImageLabel", gameTiles);

        var mediatorMock = new Mock<IMediator>();


        var mockImageService = new Mock<IImageService>();
        mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1); 
        mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto);

        var randomOracleService = new RandomOracleService(mockImageService.Object, context);

        var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

        var request = new VerifyGuess.Request(game.Id, guess);

        var handler = new VerifyGuess.Handler(context, randomOracleService);

        var result = await handler.Handle(request, CancellationToken.None);


        Assert.True(result.Success);
        Assert.True(result.IsGuessCorrect);
    }

    [Theory]
    [InlineData("WrongGuess")]
    [InlineData("wrongguess")]
    [InlineData("")]
    [InlineData("ImageLabel 1")]
    public async Task VerifyWrongGuess_ShouldReturnTrueFalse(string guess)
    {
        var image = new GuessingGame.Core.Domain.Image.Image("source", new List<Tile>(), "ImageLabel");

        var game = new GuessingGame.Core.Domain.Game.Game { Image = new GameImage { ImgSource = "source", Label = "ImageLabel", Tiles = new List<GameTile>(), ImageId = 1 } };
        using var context = new GameContext(ContextOptions, null);
        context.Database.Migrate();

        await context.Game.AddAsync(game);
        await context.SaveChangesAsync();

        var imagedto = new ImageDto(1, "Src", "ImageLabel", new List<Tile>());

        var mediatorMock = new Mock<IMediator>();


        var mockImageService = new Mock<IImageService>();
        mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1); // return 5 for image count
        mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imagedto); // return image with id 1

        var randomOracleService = new RandomOracleService(mockImageService.Object, context);

        var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

        var request = new VerifyGuess.Request(game.Id, guess);

        var handler = new VerifyGuess.Handler(context, randomOracleService);

        var result = await handler.Handle(request, CancellationToken.None);


        Assert.True(result.Success);
        Assert.False(result.IsGuessCorrect);


    }
}

