// Function to handle fragment click and invoke SendTileSelection
function handleFragmentClick(event) {
    console.log(event.offsetX, event.offsetY);
    document.getElementById('xRelativeInput').value = event.offsetX;
    document.getElementById('yRelativeInput').value = event.offsetY;
    document.getElementById('myForm').submit();

    SendTileSelection();
}


function resizeContainer(image) {
    var container = document.getElementById("image-container");
    var width = image.clientWidth;
    var height = image.clientHeight;
    container.style.height = height + "px";
    container.style.width = width + "px";
}

let lobbyMusic = document.getElementById("lobbyMusic");
lobbyMusic.play();