using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuessingGame.Core.Domain.User.Pipelines;
using GuessingGame.Core.Domain.User;
using System.ComponentModel.DataAnnotations;

namespace GuessingGame.Pages;

public class RegisterModel : PageModel
{
    private readonly IMediator _mediator;

    [BindProperty]
    public InputModel Input { get; set; }
    public string ReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public RegisterModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void OnGet(string returnUrl)
    {
        ReturnUrl = returnUrl ?? "/Index";
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userData = new UserData(Input.Username, Input.Password);
        var response = await _mediator.Send(new RegisterUser.Request(userData));

        if (response.IsSuccess)
        {
            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/Index");
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