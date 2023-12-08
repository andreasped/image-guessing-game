using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Results;

public class GetLeaderboard
{
	public record Request : IRequest<List<Leaderboard>> { }

	public class Handler : IRequestHandler<Request, List<Leaderboard>>
	{
		private readonly GameContext _db;

		public Handler(GameContext db)
		{

			_db = db ?? throw new ArgumentNullException(nameof(db));
		}

		public async Task<List<Leaderboard>> Handle(Request request, CancellationToken cancellationToken)
			=> await _db.Leaderboards.ToListAsync(cancellationToken: cancellationToken);
	}
}