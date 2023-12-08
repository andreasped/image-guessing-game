using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
    public class JoinActiveGame
    {
        public record Request(int GameId, Guid UserId) : IRequest<Unit> { }

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
                Game game = await _db.Game.Include(game => game.Image)
                .Include(game => game.Image.Tiles)
                .FirstOrDefaultAsync(g => g.Id == request.GameId, cancellationToken);
                await _gameService.JoinGame(game, request.UserId);

                return Unit.Value;
            }
        }
    }
}