using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Dto;
using GuessingGame.Core.Domain.Image;
using static GuessingGame.Core.Domain.Game.Pipelines.VerifyGuess;

namespace GuessingGame.Core.Domain.Game.Services
{

public interface IOracleService
{
    Task<ImageDto> SelectImage();
    Task<bool> AddNewTileToGame(Game game);
    Task<Response> VerifyGuess(Game game, string userGuess);
}
}