using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Dto;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static GuessingGame.Core.Domain.Game.Pipelines.VerifyGuess;

namespace GuessingGame.Core.Domain.Game.Services;

public class RandomOracleService : IOracleService
{
    private readonly IImageService _imageService;
    private readonly GameContext _db;
    public RandomOracleService(IImageService imageService, GameContext db)
    {
        _imageService = imageService;
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public async Task<ImageDto> SelectImage()
    {
        return await _imageService.GetImage(GetRandomImageId().Result);
    }

    public async Task<bool> AddNewTileToGame(Game game)
    {
        List<GameTile> unusedTiles = game.GetUnusedTiles();
        if (unusedTiles.Count() == 0) return false;

        game.AddTile(GetRandomUnusedTile(unusedTiles).Result);
        await _db.SaveChangesAsync();

        return true;
    }
    public async Task<Response> VerifyGuess(Game game, string userGuess)
    {
        if (userGuess == null) return new Response(false, false);
        game.AddGuess();
        await _db.SaveChangesAsync();
        if (game.Image.Label.ToLower() == userGuess.ToLower().Trim()) return new Response(true, true);
        return new Response(true, false);
    }

    private async Task<int> GetRandomImageId()
    {
        int totalImageCount = await _imageService.GetImageCount();
        return new Random().Next(1, totalImageCount + 1);
    }

    private async Task<GameTile> GetRandomUnusedTile(List<GameTile> unusedTiles)
    {
        int randomIndex = new Random().Next(0, unusedTiles.Count);
        return unusedTiles[randomIndex];
    }
}