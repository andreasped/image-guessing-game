using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Image.Pipelines
{
    public class GetImage
    {
        public record Request(int Id) : IRequest<Image> { }

        public class Handler : IRequestHandler<Request, Image>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Image> Handle(Request request, CancellationToken cancellationToken)
            {
                var image = await _db.Images.Include(image => image.Tile).SingleOrDefaultAsync(o => o.Id == request.Id, cancellationToken);
                if (image == null) throw new ArgumentNullException(nameof(image));
                return image;
            }
        }
    }
}