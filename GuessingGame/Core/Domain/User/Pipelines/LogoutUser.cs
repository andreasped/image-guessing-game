using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GuessingGame.Core.Domain.User.Pipelines;
public class LogoutUser
{
    public record Request() : IRequest;

    public class Handler : IRequestHandler<Request, Unit>
    {
        private readonly SignInManager<UserIdentity> _signInManager;

        public Handler(SignInManager<UserIdentity> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<Unit> Handle(Request request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            return Unit.Value;
        }
    }
}