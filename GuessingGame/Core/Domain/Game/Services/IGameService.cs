using MediatR;
using GuessingGame.Core.Domain.Game.Dto;

namespace GuessingGame.Core.Domain.Game.Services
{
    public interface IGameService
    {
        Task<Unit> CreateGame(Guid userId, GameType type);
        Task<Unit> JoinGame(Game game, Guid userId);
        Task<Unit> EndGame(Game gameDto);
        Task<Unit> SaveScoreAndExit(Game gameDto);
        Task<Unit> GiveUp(Game gameDto);
    }
}