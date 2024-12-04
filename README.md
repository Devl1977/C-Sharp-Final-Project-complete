# MemoryGame
# Overview
### This project was inspired by a suggestion from my professor during our C# class. We were encouraged to come up with project ideas based on what we learned. Drawing from an earlier Java class where we built a memory card flip game, I decided to expand that concept for C#, utilizing the WPF framework.

### The journey was challenging, fun, and occasionally frustrating, but it led to a project I'm proud to present. Here's a summary of what I've built and lessons learned along the way.

# Key Features
### Gameplay
### The MemoryGame is a card-flipping game with three difficulty levels:

- Easy: Features a small grid with car images.
- Medium: Larger grid with flower images (expanded on the idea from my Java project).
- Hard: Full 54-card deck (52 standard cards + 2 jokers).
### The game uses a grid layout that dynamically adjusts based on the selected difficulty level. Players are challenged to match all cards within a 1-minute time limit.

# Additional Gameplay Mechanics:

1. Hurry-Up Mode: When the timer reaches 10 seconds, the screen flashes, and a sound plays to increase tension.
2. Win/Loss Alerts:
3. Win: Displays a pop-up message, plays a victory sound, and transitions to a fireworks celebration screen for 2 seconds.
4. Lose: Displays a pop-up message, plays a unique defeat sound, and shows a sad face screen for 2 seconds.
5. High Scores
6. The game tracks how quickly players can match all the cards. Inspired by classic arcade games like Galaga or Ms. Pac-Man, high scores add a competitive edge. Scores can be reset by overwriting them with an empty list in the text file (or by deleting the file entirely).

# Planned Features
### I left several elements open for future updates to expand and improve the game. Here are some ideas I'm considering:

1. Card Customization: Allow players to select different card backs (currently, there's only one blue card back).
2. Multiplayer Mode: Enable a turn-based mode where players take turns flipping cards.
3. Themes and Dark Mode: Add themes with different sounds and colors, as well as a toggle for dark mode.
4. Timer Adjustment: Allow players to modify the countdown timer to make the game easier or harder.
5. Hurry-Up Customization: Let players choose the screen flash color or sound for the hurry-up phase.
6. Known Improvements
7. While testing the game, I identified some areas for refinement:

8. Reset Functionality: Streamline the reset feature, possibly by adding a dedicated reset button and locking the start button during an active game session.
9. More Sounds: Add audio feedback for successful matches and mismatches.
10. Card Size Adjustments: The cards are a bit small, so resizing them within the grid layout could improve the user experience.

# Lessons Learned
### One major takeaway from this project was the importance of frequent commits and pushing changes to GitHub. I often forgot to commit during development, which made managing my progress more difficult. This is something I plan to improve in future projects.

# Final Thoughts
### Building this game has been an incredible learning experience. While it’s not perfect, I’m excited to continue refining and expanding it. I hope you enjoy playing MemoryGame as much as I enjoyed creating it!


