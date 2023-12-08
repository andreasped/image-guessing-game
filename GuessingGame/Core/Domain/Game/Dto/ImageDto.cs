using GuessingGame.Core.Domain.Image;

namespace GuessingGame.Core.Domain.Game.Dto;

public record ImageDto
(
    int Id,
    string ImgSource,
    string Label,
    List<Tile> Tile
);