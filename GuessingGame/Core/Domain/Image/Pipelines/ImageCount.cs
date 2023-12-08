using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Image.Pipelines
{
    public class ImageCount
    {
        public record Request : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                return await _db.Images.CountAsync(cancellationToken);
            }
        }
    }
}