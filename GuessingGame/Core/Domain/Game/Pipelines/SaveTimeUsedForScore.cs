using GuessingGame.SharedKernel;
using MediatR;
using GuessingGame.Core.Domain.Game.Services;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
    public class SaveTimeUsedForScore
    {
        public record Request(int percent_of_total_time, TimeSpan timeUsed, Game game) : IRequest<Unit> { }

        public class Handler : IRequestHandler<Request, Unit>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                request.game.TimeUsedInPercent = request.percent_of_total_time;
                request.game.TimeUsed = request.timeUsed;
                await _db.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}