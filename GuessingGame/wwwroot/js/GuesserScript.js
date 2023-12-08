document.addEventListener("DOMContentLoaded", function () {
    var flashMessage = document.getElementById("flashMessage");
    if (flashMessage) {
        flashMessage.style.display = "block";
        setTimeout(function () {
            flashMessage.style.display = "none";
        }, 3000); // Message will be displayed for 3 seconds
    }
});


window.onload = function () {
    const canvas = document.getElementById("myCanvas");
    const ctx = canvas.getContext("2d");
    const images = tilesData;
    const loadedImages = [];

    for (let i = 0; i < images.length; i++) {
        let img = new Image();
        img.src = "data:image/png;base64," + images[i].source;
        img.onload = function () {
            loadedImages.push(img);
            let newWidth, newHeight;
            let SIZE = 400;
            if (loadedImages.length === images.length) {
                canvas.width = loadedImages[0].width
                canvas.height = loadedImages[0].height
                if (canvas.width > SIZE && canvas.height > SIZE || canvas.width < SIZE && canvas.height < SIZE) {
                    if (canvas.width > canvas.height) {
                        newWidth = SIZE;
                        newHeight = (canvas.height * SIZE) / canvas.width;
                    } else {
                        newHeight = SIZE;
                        newWidth = (canvas.width * SIZE) / canvas.height;
                    }
                    canvas.width = newWidth;
                    canvas.height = newHeight;
                } else {
                    newWidth = loadedImages[0].width;
                    newHeight = loadedImages[0].height;
                }
                drawImages(newWidth, newHeight);
            }
        }
    }

    function drawImages(Width, Height) {
        for (let i = 0; i < loadedImages.length; i++) {
            ctx.drawImage(loadedImages[i], 0, 0, Width, Height);
        }
    }
}

// clear local storage
function clearLocalStorage() {
    localStorage.clear();
}

// Function to get the initial time from localStorage
function getInitialTime() {
    const storedTime = localStorage.getItem('countdownTimer');
    return storedTime ? parseInt(storedTime, 10) : secondsForTimer;
}

let timeInSeconds = getInitialTime();

const timerElement = document.getElementById('timer');
const progressBarInner = document.getElementById('progress-bar-inner');

// Function to update the timer and progress bar
function updateTimer() {
    const minutes = Math.floor(timeInSeconds / 60);
    const seconds = timeInSeconds % 60;

    timerElement.innerText = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

    const progress = (timeInSeconds / secondsForTimer) * 100;
    progressBarInner.style.width = `${progress}%`;

    if (timeInSeconds === 0) {
        clearInterval(timerInterval);
        localStorage.clear();
        alert('Time is up!');
        var endGameBtn = document.getElementById("GiveUpBtn");
        endGameBtn.click();
    } else {
        timeInSeconds--;
        localStorage.setItem('countdownTimer', timeInSeconds.toString());
    }
}

updateTimer();

// Set up the timer interval (1000 milliseconds = 1 second)
const timerInterval = setInterval(updateTimer, 1000);