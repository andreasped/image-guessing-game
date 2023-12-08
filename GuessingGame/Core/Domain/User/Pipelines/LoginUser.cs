using GuessingGame.Core.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GuessingGame.Core.Domain.User.Pipelines;
public class LoginUser
{
    public record Request(UserData LoginData) : IRequest<UserResponse>;

    public class Handler : IRequestHandler<Request, UserResponse>
    {
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;

        public Handler(UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<UserResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.LoginData.UserName);
            var err = new List<string>
                {
                    "Username or password is wrong"
                };
            if (user is null)
            {
                return new UserResponse(false, err.ToArray());
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.LoginData.Password, false, false);
            if (!result.Succeeded)
            {
                return new UserResponse(false, err.ToArray());
            }

            return new UserResponse(result.Succeeded, Array.Empty<string>());
        }
    }
}