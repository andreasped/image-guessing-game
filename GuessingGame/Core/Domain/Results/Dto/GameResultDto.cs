using GuessingGame.Core.Domain.Game;

namespace GuessingGame.Core.Domain.Results.Dto;

public record GameResultDto
(
    int Id,
    Guid GuesserId,
    Guid ProposerId,
    int Score,
    DateTime dateTime,
    GameType GameType,
    int ImageId,
    TimeSpan TimeUsed,
    int GuessCount
);