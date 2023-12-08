namespace GuessingGame.Core.Domain.User
{
    public record UserResponse(bool IsSuccess, string[] Errors);
}