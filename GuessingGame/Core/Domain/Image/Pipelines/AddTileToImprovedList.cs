using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Image.Pipelines
{
    public class AddTileToImprovedList
    {
        public record Request(int ImageId, int TileId) : IRequest<Unit>;

        public class Handler : IRequestHandler<Request, Unit>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
            {
                var image = await _db.Images.SingleOrDefaultAsync(o => o.Id == request.ImageId, cancellationToken);
                if (image == null) return Unit.Value;

                image.AddToTemporaryTileList(request.TileId);

                await _db.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}