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
using GuessingGame.Core.Domain.Results.Dto;
using GuessingGame.Core.Domain.Game.Pipelines;
using System.Threading;
using GuessingGame.Core.Domain.Result.Events;
using GuessingGame.Core.Domain.Result.Handlers;
using GuessingGame.Core.Domain.Results;


namespace GuessingGameTest.Domain.Game
{
    public class GameLifeCycleIntegrationTest : DbTest
    {
        public GameLifeCycleIntegrationTest(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task TestSinglePlayerGameFromStartGameToEndGame()
        {
            //Tried to make a End to End test of the game, but it is not working as intended.
            //We the EndedGameHandler in Result is not picking up the event from the GameService

            //Arrange
            using (var context = new GameContext(ContextOptions, null))
            {

                context.Database.Migrate();
                var mediatorMock = new Mock<IMediator>();
                var userid = Guid.NewGuid();
                var gameType = GameType.SinglePlayer;
                var imageDTO = new ImageDto(1, "ImageSource", "ImageLabel", new List<Tile>{ new Tile("Tile1")});

                //Mock Image Service
                var mockImageService = new Mock<IImageService>();
                mockImageService.Setup(service => service.GetImageCount()).ReturnsAsync(1);
                mockImageService.Setup(service => service.GetImage(It.IsAny<int>())).ReturnsAsync(imageDTO);

            

                //Create Services
                var randomOracleService = new RandomOracleService(mockImageService.Object, context);
                var gameService = new GameService(context, mediatorMock.Object, randomOracleService);

                // Act
                await gameService.CreateGame(userid, gameType);
                
                var AddTileRequest = new AddNewTileToGame.Request(userid);
                var AddTileHandler = new AddNewTileToGame.Handler(context, randomOracleService);
                var AddTileResult = await AddTileHandler.Handle(AddTileRequest, CancellationToken.None);

                var request = new GetGame.Request(userid);
                var handler = new GetGame.Handler(context);
                var gameresult = await handler.Handle(request, CancellationToken.None);

                gameresult.TimeUsedInPercent = 10;
                gameresult.GuessCount = 1;

                await context.SaveChangesAsync();

                await gameService.SaveScoreAndExit(gameresult);

                var gameResultDto = new GameResultDto
                (
                    gameresult.Id,  
                    gameresult.GuesserId,          
                    gameresult.ProposerId,         
                    gameresult.Score,              
                    gameresult.dateTime,           
                    gameresult.GameType,           
                    gameresult.Image.Id,   
                    gameresult.TimeUsed,
                    gameresult.GuessCount   
                );
                
                var leaderboardRequest = new GetLeaderboard.Request();
                var leaderboardHandler = new GetLeaderboard.Handler(context);
                var leaderboardResult = await leaderboardHandler.Handle(leaderboardRequest, CancellationToken.None);
                
                // Console.WriteLine("");
                // Console.WriteLine(leaderboardResult.Count); //The count should be 1, but it is 0
                // Console.WriteLine("");
            }
        }
    }
}