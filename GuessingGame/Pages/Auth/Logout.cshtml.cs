using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuessingGame.Core.Domain.User.Pipelines;
using Microsoft.AspNetCore.Authorization;

namespace GuessingGame.Pages;
[Authorize]
public class LogoutModel : PageModel
{
    private readonly IMediator _mediator;

    public LogoutModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _mediator.Send(new LogoutUser.Request());
        return RedirectToPage("/Index");
    }
}