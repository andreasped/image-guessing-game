using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuessingGame.Core.Domain.Image.Pipelines
{
    public class GetClickedImageTileByMousePos
    {
        public record Request(int xPosition, int yPosition, int imageId) : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                var image = await _db.Images.Include(image => image.Tile).SingleOrDefaultAsync(o => o.Id == request.imageId, cancellationToken);

                foreach (var tile in image.Tile)
                {
                    byte[] tileBytes = Convert.FromBase64String(tile.Source);

                    using (var imageSharpTile = SixLabors.ImageSharp.Image.Load<Rgba32>(tileBytes))
                    {
                        if (request.xPosition > imageSharpTile.Width || request.yPosition > imageSharpTile.Height)
                        {
                            throw new Exception("Out of bounds");
                        }
                        else
                        {
                            Rgba32 pixelColor = imageSharpTile[request.xPosition, request.yPosition];
                            if (pixelColor.A != 0) return tile.Id;
                        }
                    }
                }
                return 0;
            }
        }
    }
}