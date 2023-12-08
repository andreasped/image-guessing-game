using GuessingGame.Core.Domain.Results.Dto;
using GuessingGame.SharedKernel;

namespace GuessingGame.Core.Domain.Result.Events
{
    public record EndedGame : BaseDomainEvent
    {
        public EndedGame(GameResultDto gameResultDto)
        {
            GameResultDto = gameResultDto;
        }
        public GameResultDto GameResultDto { get; set; }
    }
}