const connection = new signalR.HubConnectionBuilder()
    .withUrl("/GuessingHub")
    .build();

connection.start().then(() => {
    connection.invoke("JoinLobbyGroup").catch(err => console.error(err));
});

connection.on("NewGameCreated", (userId, isCurrentUser) => {
    window.location.reload();
});

connection.on("SomeoneJoinedTheGame", () => {
    window.location.reload()
});

function RemoveFromLobbyGroup() {
    // Send a request to the Proposer for a new tile
    connection.invoke("RemoveFromLobbyGroup").catch(err => console.error(err));
}