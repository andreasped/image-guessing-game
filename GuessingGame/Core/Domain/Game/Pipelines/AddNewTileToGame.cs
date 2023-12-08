using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game.Services;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
    public class AddNewTileToGame
    {
        public record Request(Guid Id) : IRequest<bool> { }

        public class Handler : IRequestHandler<Request, bool>
        {
            private readonly GameContext _db;
            private IOracleService _oracleService;

            public Handler(GameContext db, IOracleService oracleService)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _oracleService = oracleService;
            }

            public async Task<bool> Handle(Request request, CancellationToken cancellationToken)
            {
                Game game = await _db.Game.Include(game => game.Image)
                .Include(game => game.Image.Tiles)
                .SingleOrDefaultAsync(g => g.GuesserId == request.Id || g.ProposerId == request.Id, cancellationToken) ?? throw new Exception("Game not found"); //should use SingleOrDefaultAsync
                return await _oracleService.AddNewTileToGame(game);
            }
        }
    }
}