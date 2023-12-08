using System.ComponentModel;
using System.Diagnostics;
using GuessingGame.Core.Domain.Game;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Core.Domain.User;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace GuessingGame.Pages.Lobby;

public class TwoPlayerLobby : PageModel
{
    private readonly IHubContext<GuessingHub> _hubContext;
    private readonly IMediator _mediator;
    public UserManager<UserIdentity> _userManager { get; set; }
    [BindProperty]
    public List<Core.Domain.Game.Game> Games { get; set; } = new List<Core.Domain.Game.Game>();

    public TwoPlayerLobby(IHubContext<GuessingHub> hubContext, IMediator mediator, UserManager<UserIdentity> userManager)
    {
        _hubContext = hubContext;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task OnGetAsync()
    {
        RetrieveActiveTwoPlayerGames();
    }

    public async Task<IActionResult> OnPostCreateTwoPlayerGame()
    {
        RetrieveActiveTwoPlayerGames();
        if (HasUserActiveGame())
        {
            return RedirectToPage("/Game/Proposer");
        }
        else
        {
            return await CreateNewTwoplayerGame();
        }
    }

    public async Task<IActionResult> OnPostJoinGame(int GameId)
    {
        RetrieveActiveTwoPlayerGames();
        if (HasUserActiveGame())
        {
            ViewData["Alert"] = $"You already have an active game.";
            return Page();
        }
        else
        {
            await _mediator.Send(new JoinActiveGame.Request(GameId, GetUser())); // Pass the GUID to the request
            await _hubContext.Clients.Group($"Game_{GameId}").SendAsync("GuesserJoined");
            await _hubContext.Clients.Group("Lobby").SendAsync("SomeoneJoinedTheGame");
            return RedirectToPage("/Game/TwoPlayerGuesser");
        }
    }



    private async Task<IActionResult> CreateNewTwoplayerGame()
    {
        var CreatedTwoPLayerGame = await _mediator.Send(new CreateGame.Request(GetUser(), GameType.TwoPlayer));
        if (CreatedTwoPLayerGame.Success)
        {
            await _hubContext.Clients.Group("Lobby").SendAsync("NewGameCreated");
            return RedirectToPage("/Game/Proposer");
        }
        else
        {
            ViewData["Alert"] = $"An error occured: {CreatedTwoPLayerGame.Error}";
            return Page();
        }
    }

    public bool HasUserActiveGame()
    {
        return Games.Any(game => game.ProposerId == GetUser() || game.ProposerId == GetUser());
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

    private async void RetrieveActiveTwoPlayerGames()
    {
        Games = await _mediator.Send(new GetActiveTwoPlayerGames.Request());
    }
}