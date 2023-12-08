using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
    public class GetActiveTwoPlayerGames
    {
        public record Request() : IRequest<List<Game>> { }

        public class Handler : IRequestHandler<Request, List<Game>>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<List<Game>> Handle(Request request, CancellationToken cancellationToken)
            {
                List<Game> games = await _db.Game
                .Include(game => game.Image)
                .Include(game => game.Image.Tiles)
                .Where(g => g.GameStatus == GameStatus.Waiting && g.GameType == GameType.TwoPlayer)
                .ToListAsync(cancellationToken);
                return games;
            }
        }
    }
}