using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Image;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Game.Pipelines
{

    public class GetGame
    {
        public record Request(Guid Id) : IRequest<Game> { }

        public class Handler : IRequestHandler<Request, Game>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Game> Handle(Request request, CancellationToken cancellationToken)
            {
                return await _db.Game
                .Include(game => game.Image)
                .Include(game => game.Image.Tiles)
                .FirstOrDefaultAsync(g => g.GuesserId == request.Id || g.ProposerId == request.Id, cancellationToken);
            }
        }
    }
}