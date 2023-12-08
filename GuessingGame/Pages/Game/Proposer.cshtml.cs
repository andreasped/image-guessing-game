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
public class Proposer : PageModel
{
    private readonly IHubContext<GuessingHub> _hubContext;
    private readonly IMediator _mediator;
    [BindProperty]
    public Core.Domain.Game.Game Game { get; set; }
    public UserManager<UserIdentity> _userManager { get; set; }
    [BindProperty]
    public int xRelative { get; set; }
    [BindProperty]
    public int yRelative { get; set; }
    public List<GameTile> Tiles;

    public Proposer(IHubContext<GuessingHub> hubContext, IMediator mediator, UserManager<UserIdentity> userManager)
    {
        _hubContext = hubContext;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Tiles = new List<GameTile>();
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

        var tileId = await _mediator.Send(new GetClickedTileIdByMousePos.Request(xRelative, yRelative, Game));
        await _hubContext.Clients.Group($"Game_{Game.Id}").SendAsync("ReceiveTileSelection", tileId);
        return Page();
    }


    public async Task<IActionResult> OnPostEndGame()
    {
        Game = RetrieveUpdatedGame(GetUser()).Result;
        if (Game == null) return RedirectToPage("/Error", new { errorMessage = "Game not found." });

        await _mediator.Send(new EndGame.Request(Game, EndGame.OperationType.GiveUp));
        await _hubContext.Clients.Group($"Game_{Game.Id}").SendAsync("ReceiveProposerGaveUp");
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