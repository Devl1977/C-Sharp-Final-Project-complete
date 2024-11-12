I made the mistake of forgetting to continually committing and publishing to github while working on this project.
I know i should be better at doing that, i'm still getting used to having to push my work outside of a program im currently working on.
I will do better.

This project is one where I came up with the idea during class.  Our Professor suggested that we form our own ideas of what was taught during our c# class 
and use them in our project.  
I took the idea from our Java class where our teacher made a memory game (card flip).  I took that idea and expanded on it for our c# class utilizing the WPF format.
It was challenging, fun, frustrating at times, but here is the end look. 

The MemoryGame, that i present is a card flip game with different difficulty levels.
Easy: being images of cars 
Medium: greater number than easy mode and the images are of flowers (ideaa taken from my Java class but expanded with more images).
Hard: 54 card deck (52 but with two jokers, black and red).

I used a grid for the card game that would change based on the difficulty level selected. 
The time limit for the game is always set to 1 minute (this is easily changed in the code but i wanted to keep it consistent so it made things harder depending on the level you progressed to).
There is a hurry up element where the counter reaches 10 seconds the screen will flash and a sound will play.

When you win there is an alert (messagebox) that will popup saying that you won a sound will play unique to winning, and then another screen that shows fireworks and a message for two sends. 
When you lose you will get an alert much the same as winning that indicates you lost, a sound will play unique to losing, and another screen will show a sad face and that you lost, for two sends.

There is an element of highscores (how quickly someone can flip all the cards correctly), this idea came to me from old arcade games like Galaga or Ms Pacman.  
You can clear the scores by having the file overwrite an empty list, or you can completely delete the txt file.  I went with the former.

There are many elements to also increase into the game that i left open where i can work on at a later point :
For example: 

Adding an option to give the player on changing the back fo the card (currently there is only the one back of the card - blue)
Giving the opetion for multi player so that when one person is done the other player is prompted to take the controls.
Giving the ability (Toggle) for dark mode.  (Currently i went with a nuetral grey background), maybe themes even, where you can have sounds and colors.  Not sure yet.
Giving the ability to change the countdown timer to make things more difficult or easier.
Giving the ability to choose the color for the hurry up element.

Also, after testing this a few more options popped up:
Will want to have the reset functionaility re-worked so that it's more streamlined.  Meaning possibly having a reset button, and locking the start game from being pressed while inGameSession is true.
More Sounds:  Card flip Successfully mantch and Card Flips that do not match.
Updating the size of the cards as they are a bit small.  Would have to re-work them in with the Card Grid area, but should be doable. 

Just things i am thinking of and playing around with.  
