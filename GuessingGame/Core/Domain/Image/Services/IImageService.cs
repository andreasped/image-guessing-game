using GuessingGame.Core.Domain.Game.Dto;

public interface IImageService
{
    Task<int> GetImageCount();
    Task<ImageDto> GetImage(int imageId);
}
