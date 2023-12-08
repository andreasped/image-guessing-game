using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Result.Events;
using GuessingGame.Core.Domain.Results;
using GuessingGame.SharedKernel;
using MediatR;

namespace GuessingGame.Core.Domain.Result.Handlers
{
    public class GameEndedHandler : INotificationHandler<EndedGame>
    {
        private readonly GameContext _db;

        public GameEndedHandler(GameContext db)
        {
            _db = db;
        }

        public async Task Handle(EndedGame notification, CancellationToken cancellationToken)
        {
            var resultType = ResultType.Undefined;
            if (notification.GameResultDto.GameType == GameType.TwoPlayer)
            {
                resultType = ResultType.TwoPlayer;
            }
            else
            {
                resultType = ResultType.SinglePlayer;
            }
            var gameResult = new Leaderboard(
                notification.GameResultDto.Id,
                notification.GameResultDto.GuesserId,
                notification.GameResultDto.ProposerId,
                notification.GameResultDto.Score,
                resultType,
                notification.GameResultDto.dateTime,
                notification.GameResultDto.ImageId,
                notification.GameResultDto.TimeUsed,
                notification.GameResultDto.GuessCount
            );
            await _db.AddAsync(gameResult);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}