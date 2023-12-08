
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/guessingHub")
    .build();

connection.start().then(() => {
    // Connection established
    connection.invoke("JoinGameGroup", gameId).catch(err => console.error(err));
});

connection.on("ReceiveTileSelection", (selectedTile) => {
    // Handle the received tile selection
    alert("The Proposer chose a new tile.");
    window.location.reload();
    // You can perform any actions here based on the selected tile
});

connection.on("ReceiveProposerGaveUp", () => {
    alert("The Proposer gave up:( You will now be redirected to the menu page.");
    window.location.href = '/Menu/GameMenu';
});

function requestNewTile(gameId) {
    // Send a request to the Proposer for a new tile
    connection.invoke("RequestNewTile", gameId).catch(err => console.error(err));
};