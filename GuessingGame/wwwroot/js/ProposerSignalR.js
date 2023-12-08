const connection = new signalR.HubConnectionBuilder()
    .withUrl("/guessingHub")
    .build();

connection.on("ReceiveNewTileRequest", async () => {
    alert("The Guesser requested a new tile. Please click on the tile you want to show to the Guesser.");
    const imageFragments = document.getElementsByClassName("image-fragment");
    for (let i = 0; i < imageFragments.length; i++) {
        imageFragments[i].addEventListener("click", handleFragmentClick);
    }
});

connection.start().then(() => {
    // Connection established
    connection.invoke("JoinGameGroup", gameId).catch(err => console.error(err));
});

connection.on("ReceiveGuesserGaveUp", async () => {
    alert("The Guesser gave up:( You will now be redirected to the menu page.");
    window.location.href = '/Menu/GameMenu';
});

connection.on("ReceiveWon", async () => {
    alert("The Guesser manage to guess correctly! You will now be redirected to the menu page.");
    window.location.href = '/Menu/GameMenu';
});

connection.on("GuesserJoined", async () => {
    alert("A Guesser joined the game! Please click on the first tile to show to the Guesser.");

    const divToRemove = document.getElementById("GuesserFalse row");
    if (divToRemove) {
        divToRemove.style.display = "none";
    }
    const imageFragments = document.getElementsByClassName("image-fragment");
    for (let i = 0; i < imageFragments.length; i++) {
        imageFragments[i].addEventListener("click", handleFragmentClick);
    }
});

function SendTileSelection() {
    connection.invoke("SendTileSelection", selectedTile).catch(err => console.error(err));
}