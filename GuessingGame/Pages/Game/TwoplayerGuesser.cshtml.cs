using GuessingGame.Core.Domain.Game.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;
using GuessingGame.Core.Domain.Image.Pipelines;
using System.Security.Cryptography.X509Certificates;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Game.Pipelines;
using GuessingGame.Core.Domain.Game;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using GuessingGame.Core.Domain.User;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace GuessingGame.Pages.Game;

[Authorize]
public class TwoPlayerGuesserModel : PageModel
{
    private readonly IHubContext<GuessingHub> _hubContext;
    private readonly IMediator _mediator;
    public List<GameTile> Tiles;
    public UserManager<UserIdentity> _userManager { get; set; }
    [BindProperty]
    public Core.Domain.Game.Game Game { get; set; }
    public DateTime stopTime { get; set; }
    public TimeSpan timeUsed { get; set; }
    [BindProperty]
    public int SECONDS_FOR_TIMER { get; set; } = 180; // Sets total time (in seconds) for the timer, both frontend and backend.

    public TwoPlayerGuesserModel(IHubContext<GuessingHub> hubContext, IMediator mediator, UserManager<UserIdentity> userManager)
    {
        _hubContext = hubContext;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Tiles = new List<GameTile>(); // Initialize the Tiles list only once
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task OnGetAsync()
    {
        RetrieveUpdatedGame(GetUser());
    }

    public async Task OnPostAsync()
    {
        RetrieveUpdatedGame(GetUser());
    }

    public async Task OnPostVerifyGuess(string UserGuess)
    {
        RetrieveUpdatedGame(GetUser());
        var checkedGuess = await _mediator.Send(new VerifyGuess.Request(Game.Id, UserGuess));

        if (Game.HasUsedAllTiles() && Game.HasReachedMaxGuesses())
        {
            ViewData["Alert"] = "You did not manage to guess correctly.";
            await EndGameOperation(EndGame.OperationType.GiveUp);
        }
        else if (Game.HasReachedMaxGuesses())
        {
            ViewData["Alert"] = "You had 3 consecutive guesses for the same tile. Wait for the Proposer to give you a new tile.";
            await _hubContext.Clients.Group($"Game_{Game.Id}").SendAsync("ReceiveNewTileRequest");
        }
        else
        {
            if (checkedGuess.IsGuessCorrect)
            {
                await _hubContext.Clients.Group($"Game_{Game.Id}").SendAsync("ReceiveWon");
                CalculateTimeUsed();
                ViewData["Alert"] = "Correct Answer! Please click the 'Save Score and Return to Menu' button to end the game!";
                ShowAllTiles();
            }
            else
            {
                ViewData["Alert"] = "Incorrect Answer";
            }
        }
    }

    public async Task<IActionResult> OnPostSaveScoreAndExit()
    {
        return await EndGameOperation(EndGame.OperationType.SaveScoreAndExit);
    }

    public async Task<IActionResult> OnPostGiveUp()
    {
        return await EndGameOperation(EndGame.OperationType.GiveUp);
    }

    private async Task<IActionResult> EndGameOperation(EndGame.OperationType operationType)
    {
        RetrieveUpdatedGame(GetUser());
        if (Game == null) return RedirectToPage("/Error", new { errorMessage = "Game not found." });

        var request = new EndGame.Request(Game, operationType);
        await _mediator.Send(request);
        await _hubContext.Clients.Group($"Game_{Game.Id}").SendAsync("ReceiveGuesserGaveUp");
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

    private void RetrieveUpdatedGame(Guid UserId)
    {
        Game = _mediator.Send(new GetGame.Request(UserId)).Result;
        if (Game == null)
        {
            RedirectToPage("/Error", new { errorMessage = "Game not found." });
        }
    }
    private async Task<bool> AddNewTile()
    {
        return await _mediator.Send(new AddNewTileToGame.Request(GetUser()));
    }
    private void ShowAllTiles()
    {
        Game.UsedTiles = Game.UsedTiles.Union(Game.Image.Tiles).ToList();
    }

    private async void CalculateTimeUsed()
    {
        stopTime = DateTime.Now;
        timeUsed = stopTime - Game.dateTime;
        var seconds = (int)Math.Floor((double)timeUsed.TotalSeconds);
        int percent_of_total_time = (int)Math.Floor((double)seconds / SECONDS_FOR_TIMER * 100);

        await _mediator.Send(new SaveTimeUsedForScore.Request(percent_of_total_time, timeUsed, Game));
    }
}