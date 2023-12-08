using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Game.Pipelines;

public class VerifyGuess
{
    public record Request(int Id, string UserGuess) : IRequest<Response> { }
    public record Response(bool Success, bool IsGuessCorrect);
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly GameContext _db;
        private IOracleService _OracleService;
        public Handler(GameContext db, IOracleService OracleService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _OracleService = OracleService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var game = await _db.Game.Include(game => game.Image).FirstOrDefaultAsync(g => g.Id == request.Id, cancellationToken);
            if (game == null) return new Response(false, false);

            return await _OracleService.VerifyGuess(game, request.UserGuess);

        }
    }
}