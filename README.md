# Project Image Guessing Game

## How to start the game once you have the files locally

### You need to have Docker Engine or Docker Desktop installed to be able to run

### 1. First time startup:
Download the zip file or clone the repository from the main branch.

From the terminal in **./HansFriBerPedHaal/GuessingGame>** use the command: ```docker compose up``` to build the image and container.

The app should start on localhost and can be accessed from your browser (Microsoft Edge not recommended).

Stop the app using the command: ```docker stop guessinggame```


### 2: Subsequent startups after having done step 1 once:
From the terminal use the command: ```docker start guessinggame```


### 3: Deleting the container and image when you no longer need it:
First remove the container using the command: ```docker rm guessinggame```

Then remove the image using the command: ```docker image rm ggimage```

## How to play a Two Player Game
- You will need two different browsers open, with two different users logged in on each of them
