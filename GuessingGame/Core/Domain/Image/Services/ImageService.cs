using GuessingGame.Core.Domain.Game.Dto;
using GuessingGame.Core.Domain.Image.Pipelines;
using MediatR;

public class ImageService : IImageService
{
    private readonly IMediator _mediator;

    public ImageService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<int> GetImageCount()
    {
        return await _mediator.Send(new ImageCount.Request());
    }

    public async Task<ImageDto> GetImage(int imageId)
    {
        GuessingGame.Core.Domain.Image.Image image = await _mediator.Send(new GetImage.Request(imageId));

        return new ImageDto(image.Id, image.ImgSource, image.Label, image.Tile);

    }
}