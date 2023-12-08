using GuessingGame.SharedKernel;
using MediatR;

namespace GuessingGame.Core.Domain.Game.Pipelines
{
    public class GetClickedTileIdByMousePos
    {
        public record Request(int xPosition, int yPosition, Game game) : IRequest<int> { }

        public class Handler : IRequestHandler<Request, int>
        {
            private readonly GameContext _db;

            public Handler(GameContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<int> Handle(Request request, CancellationToken cancellationToken)
            {
                foreach (var tile in request.game.Image.Tiles)
                {
                    byte[] imageBytes = Convert.FromBase64String(tile.Source);

                    using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(imageBytes))
                    {
                        if (request.xPosition > image.Width || request.yPosition > image.Height)
                        {
                            throw new Exception("Out of bounds");
                        }
                        else
                        {
                            Rgba32 pixelColor = image[request.xPosition, request.yPosition];
                            if (pixelColor.A != 0 && request.game.UsedTiles.Contains(tile) == false)
                            {
                                request.game.AddTile(tile);
                                request.game.ResetConsecutiveGuessCount();
                                await _db.SaveChangesAsync();
                                return tile.Id;
                            }
                        }
                    }
                }
                return 0;
            }
        }
    }
}