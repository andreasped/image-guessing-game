using MediatR;
using Microsoft.AspNetCore.Identity;

namespace GuessingGame.Core.Domain.User.Pipelines;
public class RegisterUser
{
    public record Request(UserData RegisterData) : IRequest<UserResponse>;

    public class Handler : IRequestHandler<Request, UserResponse>
    {
        private readonly UserManager<UserIdentity> _userManager;

        public Handler(UserManager<UserIdentity> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var user = new UserIdentity
            {
                UserName = request.RegisterData.UserName
            };
            var result = await _userManager.CreateAsync(user, request.RegisterData.Password);
            var errList = new List<string>();
            foreach (var err in result.Errors)
            {
                errList.Add(err.Description);
            }

            return new UserResponse(result.Succeeded, errList.ToArray());
        }
    }
}