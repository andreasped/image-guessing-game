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
using Microsoft.AspNetCore.Authorization;
using System.Security.AccessControl;

namespace GuessingGame.Pages.Game;

[Authorize]
public class SinglePlayerGuesserModel : PageModel
{
    private readonly IMediator _mediator;
    [BindProperty]
    public Core.Domain.Game.Game Game { get; set; }
    public List<GameTile> Tiles;
    public UserManager<UserIdentity> _userManager { get; set; }
    public DateTime stopTime { get; set; }
    public TimeSpan timeUsed { get; set; }
    [BindProperty]
    public int SECONDS_FOR_TIMER { get; set; } = 180; // Sets total time (in seconds) for the timer, both frontend and backend.
    [BindProperty]
    public string UserGuess { get; set; }

    public SinglePlayerGuesserModel(IMediator mediator, UserManager<UserIdentity> userManager)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Tiles = new List<GameTile>();
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task OnGetAsync()
    {
        await AddNewTile();
        RetrieveUpdatedGame(GetUser());
    }

    public async Task OnPostNextTile()
    {
        bool isSuccess = AddNewTile().Result;
        if (!isSuccess)
        {
            ViewData["Alert"] = "There are now no more tiles to show. You have 3 more guesses.";
        }
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
            await AddNewTile();
            RetrieveUpdatedGame(GetUser());
        }
        else if (checkedGuess.IsGuessCorrect)
        {
            CalculateTimeUsed();
            ViewData["Alert"] = "Correct Answer! Please click the 'Save Score and Return to Menu' button to end the game!";
            ShowAllTiles();
        }
        else
        {
            ViewData["Alert"] = "Incorrect Answer";
        }
    }

    public async Task<IActionResult> OnPostUserDefinedSequence()
    {
        return RedirectToPage("/Game/UserDefinedSequence");
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

    private async Task<bool> AddNewTile()
    {
        return await _mediator.Send(new AddNewTileToGame.Request(GetUser()));
    }

    private void RetrieveUpdatedGame(Guid UserId)
    {
        Game = _mediator.Send(new GetGame.Request(UserId)).Result;
        if (Game == null)
        {
            RedirectToPage("/Error", new { errorMessage = "Game not found." });
        }
    }

    private async void CalculateTimeUsed()
    {
        stopTime = DateTime.Now;
        timeUsed = stopTime - Game.dateTime;
        var seconds = (int)Math.Floor((double)timeUsed.TotalSeconds);
        int percent_of_total_time = (int)Math.Floor((double)seconds / SECONDS_FOR_TIMER * 100); // percent of total time.
        await _mediator.Send(new SaveTimeUsedForScore.Request(percent_of_total_time, timeUsed, Game));
    }

    private void ShowAllTiles()
    {
        Game.UsedTiles = Game.UsedTiles.Union(Game.Image.Tiles).ToList();
    }
}