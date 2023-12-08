using GuessingGame.SharedKernel;
using MediatR;
using GuessingGame.Core.Domain.Game.Services;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
    public class EndGame
    {
        public record Request(Game game, OperationType Operation) : IRequest<Unit> { }

        public enum OperationType
        {
            SaveScoreAndExit,
            GiveUp
        }
        public class Handler : IRequestHandler<Request, Unit>
        {
            private readonly GameContext _db;
            private readonly IGameService _gameService;

            public Handler(IGameService gameService, GameContext db)
            {
                _gameService = gameService;
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                switch (request.Operation)
                {
                    case OperationType.SaveScoreAndExit:
                        await _gameService.SaveScoreAndExit(request.game);
                        break;
                    case OperationType.GiveUp:
                        await _gameService.GiveUp(request.game);
                        break;
                }

                return Unit.Value;
            }
        }
    }
}