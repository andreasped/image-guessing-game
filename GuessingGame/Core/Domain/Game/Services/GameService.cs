using GuessingGame.SharedKernel;
using MediatR;
using GuessingGame.Core.Domain.Game.Dto;
using GuessingGame.Core.Domain.Results.Dto;
using GuessingGame.Core.Domain.Result.Events;

namespace GuessingGame.Core.Domain.Game.Services
{
    public class GameService : IGameService
    {
        private readonly GameContext _db;
        private readonly IMediator _mediator;
        private readonly IOracleService _oracleService;
        public Game game { get; set; }

        public GameService(GameContext db, IMediator mediator, IOracleService oracleService)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _oracleService = oracleService ?? throw new ArgumentNullException(nameof(oracleService));
        }

        public async Task<Unit> CreateGame(Guid userId, GameType type)
        {
            var oracleImage = await _oracleService.SelectImage();

            var gameTile = new List<GameTile>();

            foreach (var tile in oracleImage.Tile)
            {
                gameTile.Add(new GameTile(tile.Source));
            }

            var gameImage = new GameImage(oracleImage.ImgSource, oracleImage.Label, gameTile, oracleImage.Id);

            if (type == GameType.TwoPlayer)
            {
                game = new Game(Guid.Empty, userId, type, GameStatus.Waiting, gameImage);
            }
            else if (type == GameType.SinglePlayer)
            {
                game = new Game(userId, Guid.Empty, type, GameStatus.Active, gameImage);
            }
            await _db.Game.AddAsync(game);
            await _db.SaveChangesAsync();

            return Unit.Value;
        }

        public async Task<Unit> JoinGame(Game game, Guid userId)
        {
            game.GameStatus = GameStatus.Active;
            game.GuesserId = userId;
            await _db.SaveChangesAsync();
            return Unit.Value;
        }


        public async Task<Unit> SaveScoreAndExit(Game game)
        {
            game.CalculateScore();
            await EndGame(game);
            return Unit.Value;
        }

        public async Task<Unit> GiveUp(Game game)
        {
            game.Score = 0;
            await EndGame(game);
            return Unit.Value;
        }

        public async Task<Unit> EndGame(Game game)
        {
            var gameResultDto = new GameResultDto(
                game.Id,
                game.GuesserId,
                game.ProposerId,
                game.Score,
                game.dateTime,
                game.GameType,
                game.Image.ImageId,
                game.TimeUsed,
                game.GuessCount
            );
            game.GameStatus = GameStatus.Ended;
            _db.Remove(game);
            await _db.SaveChangesAsync();
            await _mediator.Publish(new EndedGame(gameResultDto));
            return Unit.Value;
        }
    }
}