using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class GuessingHub : Hub
{
    public async Task SendTileSelection(string selectedTile, int gameId)
    {
        await Clients.Group(GetGroupName(gameId)).SendAsync("ReceiveTileSelection", selectedTile);
    }

    public async Task RequestNewTile(int gameId)
    {
        await Clients.Group(GetGroupName(gameId)).SendAsync("ReceiveNewTileRequest");
    }

    public async Task CorrectAnswer(int gameId)
    {
        await Clients.Group(GetGroupName(gameId)).SendAsync("ReceiveWon");
    }

    public async Task ReceiveGuesserGaveUp(int gameId)
    {
        await Clients.Group(GetGroupName(gameId)).SendAsync("ReceiveGuesserGaveUp");
    }

    public async Task JoinGameGroup(int gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(gameId));
        Console.WriteLine($"Connection {Context.ConnectionId} joined group {GetGroupName(gameId)}");
    }

    public async Task JoinLobbyGroup()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
        Console.WriteLine($"Connection {Context.ConnectionId} joined group Lobby");
    }

    public async Task RemoveFromLobbyGroup()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Lobby");
        Console.WriteLine($"Connection {Context.ConnectionId} left the group Lobby");
    }

    private string GetGroupName(int gameId)
    {
        if (gameId <= 0)
        {
            throw new ArgumentException("Invalid gameId");
        }
        return $"Game_{gameId}";
    }
}