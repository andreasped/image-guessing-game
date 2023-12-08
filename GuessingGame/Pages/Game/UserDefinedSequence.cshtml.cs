using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GuessingGame.Pages.Game;

[Authorize]
public class UserDefinedSequence : PageModel
{
    private readonly IMediator _mediator;
    public UserManager<UserIdentity> _userManager { get; set; }
    [BindProperty]
    public Core.Domain.Game.Game Game { get; set; }
    [BindProperty]
    public int xRelative { get; set; }
    [BindProperty]
    public int yRelative { get; set; }

    public UserDefinedSequence(IMediator mediator, UserManager<UserIdentity> userManager)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<IActionResult> OnGetAsync()
    {
        Game = RetrieveUpdatedGame(GetUser()).Result;
        if (Game == null) return RedirectToPage("/Menu/GameMenu");

        return Page();
    }

    [HttpPost]
    public async Task<IActionResult> OnPostAsync(int xRelative, int yRelative)
    {
        Game = RetrieveUpdatedGame(GetUser()).Result;
        if (Game == null) return RedirectToPage("/Menu/GameMenu");
        var tileId = await _mediator.Send(new GetClickedImageTileByMousePos.Request(xRelative, yRelative, Game.Image.ImageId));
        await _mediator.Send(new AddTileToImprovedList.Request(Game.Image.ImageId, tileId));
        return Page();
    }

    public async Task<IActionResult> OnPostSaveScoreAndRedirect()
    {
        Game = RetrieveUpdatedGame(GetUser()).Result;
        await EndGameOperation(EndGame.OperationType.SaveScoreAndExit);
        return RedirectToPage("/Menu/GameMenu");
    }

    private async Task<IActionResult> EndGameOperation(EndGame.OperationType operationType)
    {
        Game = RetrieveUpdatedGame(GetUser()).Result;
        await _mediator.Send(new EndGame.Request(Game, operationType));
        return RedirectToPage("/Menu/GameMenu");
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

    private async Task<Core.Domain.Game.Game> RetrieveUpdatedGame(Guid UserId)
    {
        return await _mediator.Send(new GetGame.Request(UserId));
    }
}