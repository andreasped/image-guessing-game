using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuessingGame.Core.Domain.User;
using MediatR;
using GuessingGame.Core.Domain.Results;
using GuessingGame.Core.Domain.Image.Pipelines;
using Microsoft.AspNetCore.Mvc;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;

namespace GuessingGame.Pages;

[Authorize]
public class GameMenuModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly UserManager<UserIdentity> _userManager;
    public string Username { get; set; }
    public Core.Domain.Game.Game Game { get; set; }
    public List<Leaderboard> Leaderboard { get; set; } = new();
    [BindProperty]
    public IFormFile Upload { get; set; }
    [BindProperty]
    public string Label { get; set; }
    [BindProperty]
    public int GridX { get; set; } = 7;
    [BindProperty]
    public int GridY { get; set; } = 7;

    public GameMenuModel(UserManager<UserIdentity> userManager, IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            Username = user.UserName;
        }

        Leaderboard = await _mediator.Send(new GetLeaderboard.Request());
    }

    public async Task<IActionResult> OnPostRedirectToLobby()
    {
        return RedirectToPage("/Lobby/TwoPlayerLobby");
    }

    public async Task<IActionResult> OnPostSingleplayerGame()
    {
        Game = await _mediator.Send(new GetGame.Request(GetUser()));

        if (Game != null)
        {
            return RedirectToPage("/Game/SinglePlayerGuesser");
        }
        else
        {
            return await CreateNewSingleplayerGame();
        }
    }

    public async Task<IActionResult> OnPostUploadImageAsync()
    {
        if (Upload == null || Upload.Length == 0)
        {
            ModelState.AddModelError("Upload", "Please select a file to upload.");
            return Page();
        }

        var uploadedSucess = await _mediator.Send(new UserUploadImage.Request(Upload, Label, GridX, GridY));
        if (uploadedSucess.Success)
        {
            TempData["SuccessMessage"] = "Image upload successful!";
            return Page();
        }
        else
        {
            foreach (var error in uploadedSucess.Errors)
            {
                TempData["SuccessMessage"] = $"An error occured: {error}";
            }
            return Page();
        }
    }

    private async Task<IActionResult> CreateNewSingleplayerGame()
    {
        var gameCreated = await _mediator.Send(new CreateGame.Request(GetUser(), GameType.SinglePlayer));
        if (gameCreated.Success)
        {
            return RedirectToPage("/Game/SinglePlayerGuesser");
        }
        else
        {
            TempData["SuccessMessage"] = $"An error occured: {gameCreated.Error}";
            return Page();
        }
    }

    private Guid GetUser()
    {
        string userId = _userManager.GetUserId(User);

        if (!string.IsNullOrEmpty(userId))
        {
            return new Guid(userId);
        }
        else
        {
            throw new Exception("User ID is null or empty.");
        }
    }
}