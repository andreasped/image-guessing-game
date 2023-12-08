using Microsoft.AspNetCore.Identity;
using GuessingGame.Core.Domain.User.Pipelines;
using GuessingGame.Core.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GuessingGame.Pages;
public class IndexLoginModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    public UserData UserInput { get; set; }

    public string ReturnUrl { get; set; }

    public IndexLoginModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/Menu/GameMenu";
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var response = await _mediator.Send(new LoginUser.Request(UserInput));

        if (response.IsSuccess)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
        return Page();

    }
}