using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game.Services;
using MediatR;

namespace GuessingGame.Core.Domain.Game.Pipelines;
public class CreateGame
{

    public record Request(Guid UserId, GameType GameType) : IRequest<Response>;
    public record Response(bool Success, string Error);

    public class Handler : IRequestHandler<Request, Response>
    {
        private IGameService _gameService;

        public Handler(IGameService gameService)
        {
            _gameService = gameService;
        }

        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            await _gameService.CreateGame(request.UserId, request.GameType);
            return new Response(true, "");
        }
    }
}
