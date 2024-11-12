using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Windows.Threading;
using System.IO;
using Newtonsoft.Json;  // Needed to install Newtonsoft.Json package via NuGet to keep track of scores and store them.
using System.Windows.Media; // this is for the solidcolor brush that i used in the flashing scenario for hurry up.

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        private List<Card> _cards;
        private List<Button> _cardButtons;
        private int _flippedCardsCount = 0;
        private Button _firstFlippedCard;
        private Button _secondFlippedCard;
        private DispatcherTimer _countdownTimer; // The timer for the countdown
        private int _remainingTime = 60;  // 60 seconds countdown
        private int _matchedPairsCount = 0; // To keep track of the matched pairs (this is needed for player win)
        private DispatcherTimer _flashTimer;  // This is going to be for the "hurry up" scenario when coundown hits 10
        private bool _isFlashing = false;  // Boolean for the flashing scenario either flashing or not.
        private bool _isGameInSession = false; // I needed a check system for when the game is running.  (Timer countdown doubled otherwise)

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Populate the difficulty combo box
            DifficultyComboBox.Items.Add("Easy");
            DifficultyComboBox.Items.Add("Medium");
            DifficultyComboBox.Items.Add("Hard");
            DifficultyComboBox.SelectedIndex = 0; // Set the default difficulty to Easy

            _cardButtons = new List<Button>();
        }

        private void InitializeTimer()
        {
            // Stop and dispose of the existing timer if it exists
            if (_countdownTimer != null)
            {
                _countdownTimer.Stop();
                _countdownTimer = null;
            }

            // To initialize the timer and help with the countdown
            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);  // Set the interval to 1 second
            _countdownTimer.Tick += CountdownTimer_Tick;  // Attach the tick event handler
        }


        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            _remainingTime--;  // Decrease the time by 1 second

            // Update the UI to show the remaining time
            TimerLabel.Content = $"Time Left: {_remainingTime} seconds";

            if (_remainingTime == 10)
            {
                // Start the flashing effect when time hits 10 seconds
                StartFlashingBackground();
                // Play the hurry up sound same time as the flashing effect here.
                PlaySound("hurryup.wav");
            }

            if (_remainingTime == 0)
            {
                // Stop the timer when time is up
                _countdownTimer.Stop();

                StopFlashingBackground();

                // Play the Lose sound right before they get the message they lost
                PlaySound("lose.wav");

                // Optionally, show a message when time runs out
                MessageBox.Show("Time's up! Game over.");

                var gameOverWindow = new GameOverWindow
                {
                    Owner = this // Center the window relative to the main game window
                };
                gameOverWindow.ShowDialog();
                _isGameInSession = false; // Stopping the game session
            }
        }
        private void StartFlashingBackground()
        {
            if (_flashTimer == null)
            {
                _flashTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };
                _flashTimer.Tick += FlashBackground;
            }
            _flashTimer.Start();
        }

        private void FlashBackground(object sender, EventArgs e)
        {
            // Toggle the background color between light red and the original color
            if (!_isFlashing)
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 102, 102)); // Light red color
            }
            else
            {
                this.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // Original color (white)
            }

            _isFlashing = !_isFlashing;
        }

        private void StopFlashingBackground()
        {
            if (_flashTimer != null)
            {
                _flashTimer.Stop();
                this.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255)); // Reset to original color (white)
                _isFlashing = false;
            }
        }



        // Start the game based on the difficulty level selected
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if a game is already in session
            if (!CheckIfGameInSession()) return;

            // Get selected difficulty
            string selectedDifficulty = DifficultyComboBox.SelectedItem.ToString();

            // Reset the timer to 60 seconds at the start of each game
            _remainingTime = 60;

            // Initialize and start the timer
            InitializeTimer();
            _countdownTimer.Start();

            // Set the game session flag to true
            _isGameInSession = true;

            // Set the folder based on selected difficulty
            string folderName = "";
            switch (selectedDifficulty)
            {
                case "Easy":
                    folderName = "Cars";  // Easy uses Cars folder
                    break;
                case "Medium":
                    folderName = "Flowers";  // Medium uses Flowers folder
                    break;
                case "Hard":
                    folderName = "Cards";  // Hard uses Cards folder
                    break;
                default:
                    folderName = "Cars";  // Default to Cars folder
                    break;
            }

            // Use the folderName to generate the cards
            _cards = CreateCards(folderName);  // Pass folderName to CreateCards
            _flippedCardsCount = 0;

            // Clear any existing buttons in the grid
            CardGrid.Children.Clear();

            // Set up the grid layout dynamically based on the number of cards and difficulty
            SetGridLayout(_cards.Count, selectedDifficulty);  // Pass selected difficulty here

            // Calculate number of rows and columns based on selected difficulty
            int rows = 0;
            int columns = 0;

            switch (selectedDifficulty)
            {
                case "Easy":
                    rows = 4;
                    columns = 4;
                    break;
                case "Medium":
                    rows = 5;
                    columns = 6;
                    break;
                case "Hard":
                    rows = 9;
                    columns = 12;
                    break;
                default:
                    rows = 4;
                    columns = 4;
                    break;
            }

            // Create buttons for the cards
            for (int i = 0; i < _cards.Count; i++)
            {
                var cardValue = _cards[i].Value;

                // Create the button
                var button = new Button
                {
                    Width = 60,
                    Height = 90,
                    Margin = new System.Windows.Thickness(5)
                };

                // Set the card back image initially
                var cardBackUri = new Uri("pack://application:,,,/Images/Cards/CardBack.jpg", UriKind.Absolute);
                var cardBackImage = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(cardBackUri),
                    Width = 60,
                    Height = 90
                };

                button.Content = cardBackImage;

                // Attach the card value to the button's Tag property
                button.Tag = cardValue;

                // Place the button in the grid at calculated row/column
                Grid.SetRow(button, i / columns);  // Row based on the number of columns
                Grid.SetColumn(button, i % columns);  // Column based on the index

                // Attach the button click event
                button.Click += CardButton_Click;

                // Add the button to the grid
                CardGrid.Children.Add(button);
            }

            // Debugging: Output the number of buttons added
            Debug.WriteLine($"Number of buttons added to grid: {CardGrid.Children.Count}");
        }
        // check to see if the game is already in session (because previously the timer was getting messed up
        // when the game was already in session.
        private bool CheckIfGameInSession()
        {
            if (_isGameInSession)
            {
                // Pause the timer
                _countdownTimer.Stop();

                // Show a confirmation message box
                var result = MessageBox.Show("A game is already in session. Would you like to reset the game or continue?",
                                             "Game in Session", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Player chose to reset the game
                    ResetGame();
                    return false; // Allow the game to start fresh
                }
                else if (result == MessageBoxResult.No)
                {
                    // Player chose to continue, restart the timer
                    _countdownTimer.Start();
                    return false; // Do not start a new game
                }
                else
                {
                    // Player chose Cancel or closed the dialog, resume the timer
                    _countdownTimer.Start();
                    return false; // Do not start a new game
                }
            }

            // If no game is in session, proceed with starting a new game
            return true;
        }
        // This is here because if you click on start game while the game is in session i wanted there to be a "reset" type sceanrio
        private void ResetGame()
        {
            // Stop the countdown timer
            _countdownTimer.Stop();

            // Reset the timer and game variables
            _remainingTime = 60;
            TimerLabel.Content = $"Time Left: {_remainingTime} seconds";

            // Reset game state variables
            _matchedPairsCount = 0;
            _flippedCardsCount = 0;
            _firstFlippedCard = null;
            _secondFlippedCard = null;
            _isGameInSession = false;  // Resets the game session flag

            // Clear the grid
            CardGrid.Children.Clear();
        }

        private void SetGridLayout(int numberOfCards, string difficulty)
        {
            int rows = 0;
            int columns = 0;

            // Determine the number of rows and columns based on the difficulty
            switch (difficulty)
            {
                case "Easy":
                    rows = 4;
                    columns = 4;  // 4x4 grid for Easy
                    break;
                case "Medium":
                    rows = 5;
                    columns = 6;  // 6x5 grid for Medium
                    break;
                case "Hard":
                    rows = 12;
                    columns = 9;  // 9x12 grid for Hard
                    break;
                default:
                    rows = 4;
                    columns = 4;
                    break;
            }

            // Clear existing rows and columns in the grid
            CardGrid.RowDefinitions.Clear();
            CardGrid.ColumnDefinitions.Clear();

            // Add row definitions dynamically based on the selected difficulty
            for (int i = 0; i < rows; i++)
            {
                CardGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            }

            // Add column definitions dynamically (use the determined number of columns)
            for (int i = 0; i < columns; i++)
            {
                CardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Debugging output to verify the grid layout
            Debug.WriteLine($"Grid will have {rows} rows and {columns} columns.");
        }

        // Handle card button clicks
        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;  // Early return if the button is null

            var cardValue = button.Tag as string;
            if (cardValue == null) return;  // Ensure the card value is not null

            // Get the selected difficulty level from the ComboBox
            var selectedDifficulty = DifficultyComboBox.SelectedItem.ToString();

            // Determine the folder based on the difficulty level
            string folder = string.Empty;
            switch (selectedDifficulty)
            {
                case "Easy":
                    folder = "Cars";  // Easy -> use images from the "Cars" folder
                    break;
                case "Medium":
                    folder = "Flowers";  // Medium -> use images from the "Flowers" folder
                    break;
                case "Hard":
                    folder = "Cards";  // Hard -> use images from the "Cards" folder
                    break;
            }

            // Build the image path based on the selected folder
            var imagePath = $"pack://application:,,,/Images/{folder}/{cardValue.ToLower()}.jpg";
            Debug.WriteLine("Loading image from: " + imagePath);

            var cardImage = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute)),
                Width = 60,
                Height = 90
            };

            button.Content = cardImage;
            button.IsEnabled = false;  // Disable the button to prevent multiple flips

            // Handle the flipped cards
            if (_firstFlippedCard == null)
            {
                _firstFlippedCard = button;  // This is the first card flipped
            }
            else
            {
                _secondFlippedCard = button;  // This is the second card flipped
                CheckForMatch();  // After both cards are flipped, check for a match
            }
        }

        private async void CheckForMatch()
        {
            // Ensure both cards are flipped
            if (_firstFlippedCard != null && _secondFlippedCard != null)
            {
                var firstCard = _firstFlippedCard.Tag as string;
                var secondCard = _secondFlippedCard.Tag as string;

                // If the cards do not match
                if (firstCard != secondCard)
                {
                    Debug.WriteLine($"No match: {firstCard} and {secondCard}");

                    // Wait for a brief moment to let the player see the second card
                    await Task.Delay(500);  // Adjust the delay for better visual effect

                    // Flip both cards back
                    FlipCardBack(_firstFlippedCard);
                    FlipCardBack(_secondFlippedCard);
                }
                else
                {
                    // Cards match, increase matched pairs count
                    Debug.WriteLine($"Match: {firstCard} and {secondCard}");

                    _matchedPairsCount++;

                    // Check if all pairs are matched
                    if (_matchedPairsCount == _cards.Count / 2)
                    {
                        // If all pairs are matched, show "You Win!" message
                        ShowWinMessage();
                    }
                }

                // Reset flipped cards for the next pair
                _firstFlippedCard = null;
                _secondFlippedCard = null;
            }
        }
        
        private void FlipCardBack(Button button)
        {
            // Check if the button is null
            if (button == null)
            {
                Debug.WriteLine("Error: The button passed to FlipCardBack is null!");
                return;  // Exit the method to avoid NullReferenceException
            }

            // Flip the card back to the original state (back of the card)
            var cardBackUri = new Uri("pack://application:,,,/Images/Cards/CardBack.jpg", UriKind.Absolute);
            var cardBackImage = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(cardBackUri),
                Width = 60,
                Height = 90
            };

            button.Content = cardBackImage;  // Set the card back image
            button.IsEnabled = true;  // Re-enable the button
        }

        // Create cards based on the selected difficulty level
        private List<Card> CreateCards(string folderName)
        {
            var cards = new List<Card>();

            // Define the card values based on the selected folder
            List<string> cardValues;

            // Load the appropriate image folder based on the difficulty level
            switch (folderName)
            {
                case "Cars":
                    cardValues = GetImageFilesFromFolder("Cars");
                    break;
                case "Flowers":
                    cardValues = GetImageFilesFromFolder("Flowers");
                    break;
                case "Cards":
                    cardValues = GetImageFilesFromFolder("Cards");
                    break;
                default:
                    cardValues = GetImageFilesFromFolder("Cars");  // Default folder
                    break;
            }

            // Duplicate each card to create pairs
            List<string> cardPairs = new List<string>(cardValues);
            cardPairs.AddRange(cardValues);  // Add the same cards again to create pairs

            // Shuffle the cards to randomize their order
            var rand = new Random();
            var shuffledCards = cardPairs.OrderBy(x => rand.Next()).ToList();

            // Create Card objects for each shuffled card
            foreach (var cardValue in shuffledCards)
            {
                cards.Add(new Card { Value = cardValue });
            }

            return cards;
        }

        private void CreateCardsForGrid()
        {
            // Get the number of cards based on the difficulty level
            int numberOfCards = _cards.Count;

            // Set the grid layout based on the difficulty
            string difficulty = DifficultyComboBox.SelectedItem.ToString();
            SetGridLayout(numberOfCards, difficulty);

            // Create buttons for the cards dynamically
            for (int i = 0; i < _cards.Count; i++)
            {
                var cardValue = _cards[i].Value;

                // Create the button
                var button = new Button
                {
                    Margin = new System.Windows.Thickness(5)
                };

                // Adjust the card size based on the difficulty
                if (difficulty == "Easy")
                {
                    button.Width = 60;
                    button.Height = 90;
                }
                else if (difficulty == "Medium")
                {
                    button.Width = 50;
                    button.Height = 75;
                }
                else // For Hard difficulty
                {
                    button.Width = 40; // Smaller button for hard
                    button.Height = 60;
                }

                // Set the card back image initially
                var cardBackUri = new Uri("pack://application:,,,/Images/Cards/CardBack.jpg", UriKind.Absolute);
                var cardBackImage = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(cardBackUri),
                    Width = button.Width,
                    Height = button.Height
                };

                button.Content = cardBackImage;

                // Attach the card value to the button's Tag property
                button.Tag = cardValue;

                // Place the button in the grid at calculated row/column
                Grid.SetRow(button, i / 9); // 9 columns per row for hard difficulty
                Grid.SetColumn(button, i % 9); // Column index based on i for hard difficulty

                // Attach the button click event
                button.Click += CardButton_Click;

                // Add the button to the grid
                CardGrid.Children.Add(button);
            }

            // Debugging: Output the number of buttons added
            Debug.WriteLine($"Number of buttons added to grid: {CardGrid.Children.Count}");
        }

        private List<string> GetImageFilesFromFolder(string folderName)
        {
            // Hardcoded image paths from the folders
            // You might want to dynamically retrieve these images in a real-world scenario
            List<string> imageFiles = new List<string>();

            // Define a list of images per folder. These images can come from actual resources or be hardcoded.
            switch (folderName)
            {
                case "Cars":
                    imageFiles.AddRange(new List<string>
                    {
                        "AquaCar", "BlueCar", "GreenCar", "OrangeCar", "PurpleCar", "RedCar", "WarpCar",
                        "YellowCar"
                    });
                    break;

                case "Flowers":
                    imageFiles.AddRange(new List<string>
                    {
                        "flower1", "flower2", "flower3", "flower4", "flower5", "flower6", "flower7",
                        "flower8", "flower9", "flower10", "flower11", "flower12", "flower13", "flower14", "flower15"
                    });
                    break;

                case "Cards":
                    imageFiles.AddRange(new List<string>
                    {
                        "2OfClubs", "3OfClubs", "4OfClubs", "5OfClubs", "6OfClubs", "7OfClubs", "8OfClubs", "9OfClubs",
                        "10OfClubs", "JOfClubs", "QOfClubs", "KOfClubs", "AOfClubs", "2OfDiamonds", "3OfDiamonds", "4OfDiamonds",
                        "5OfDiamonds", "6OfDiamonds", "7OfDiamonds", "8OfDiamonds", "9OfDiamonds", "10OfDiamonds", "JOfDiamonds",
                        "QOfDiamonds", "KOfDiamonds", "AOfDiamonds", "2OfSpades", "3OfSpades", "4OfSpades", "5OfSpades",
                        "6OfSpades", "7OfSpades", "8OfSpades", "9OfSpades", "10OfSpades", "JOfSpades", "QOfSpades", "KOfSpades",
                        "AOfSpades", "2OfHearts", "3OfHearts", "4OfHearts", "5OfHearts", "6OfHearts", "7OfHearts", "8OfHearts",
                        "9OfHearts", "10OfHearts", "JOfHearts", "QOfHearts", "KOfHearts", "AOfHearts", "RedJoker", "BlackJoker"
                    });
                    break;

                default:
                    imageFiles = new List<string>();  // Empty list if folder is invalid
                    break;
            }

            return imageFiles;
        }

        // Adding a playerscore for traking of highscores
        public class PlayerScore
        {
            public string PlayerName { get; set; }
            public int TimeTaken { get; set; } // Time taken in seconds

            public PlayerScore(string playerName, int timeTaken)
            {
                PlayerName = playerName;
                TimeTaken = timeTaken;
            }
        }
        // a message to advise of time taken and saving to file
        private void ShowWinMessage()
        {
            // Stop the countdown timer
            _countdownTimer.Stop();

            // Capture the player’s name and time taken
            string playerName = PlayerNameTextBox.Text;
            int timeTaken = _remainingTime;

            // Create a new PlayerScore object
            var playerScore = new PlayerScore(playerName, timeTaken);

            // Save the score to the high scores list
            SaveScoreToFile(playerScore);

            // Display the updated high scores
            DisplayHighScores();

            //Play the sound immediately right before the message is shown
            PlaySound("win.wav");

            // Show a MessageBox when the game is won
            MessageBox.Show($"You Win! Time taken: {timeTaken} seconds", "Congratulations", MessageBoxButton.OK, MessageBoxImage.Information);

            // Display the fireworks animation
            var fireworksWindow = new FireworksWindow
            {
                Owner = this // Center the fireworks window relative to the main window
            };
            fireworksWindow.ShowDialog(); // Show the fireworks as a modal dialog
            _isGameInSession = false; // Stopping the game session 

        }
        // Saving scores to file
        private void SaveScoreToFile(PlayerScore playerScore)
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MemoryGame");
            string filePath = Path.Combine(directoryPath, "highscores.json");

            // Ensure the directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            List<PlayerScore> scores;

            // Load existing scores from file, or create a new list if the file doesn't exist
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                scores = JsonConvert.DeserializeObject<List<PlayerScore>>(json) ?? new List<PlayerScore>();
            }
            else
            {
                scores = new List<PlayerScore>();
            }

            // Add the new score to the list
            scores.Add(playerScore);

            // Sort the list by time taken (ascending order)
            scores = scores.OrderBy(s => s.TimeTaken).ToList();

            // Save the updated list back to the file
            string updatedJson = JsonConvert.SerializeObject(scores, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);
        }
        // Show the high schores
        private void DisplayHighScores()
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MemoryGame");
            string filePath = Path.Combine(directoryPath, "highscores.json");

            // Load the scores from the file
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var scores = JsonConvert.DeserializeObject<List<PlayerScore>>(json) ?? new List<PlayerScore>();

                // Take the top 5 scores
                var topScores = scores.Take(5).ToList();

                // Format the high scores as a string
                string scoreText = "High Scores:\n";
                foreach (var score in topScores)
                {
                    scoreText += $"{score.PlayerName}: {score.TimeTaken} seconds\n";
                }

                // Display the high scores in the StatsLabel (or another UI element)
                StatsLabel.Text = scoreText;
            }
        }
        private void ViewHighScoresButton_Click(object sender, EventArgs e)
        {
            // Create and show the high scores form
            HighScoresWindow highScoresForm = new HighScoresWindow();
            highScoresForm.ShowDialog(); // Show as a modal dialog
        }

        // This method will be called when the player wins
        private void OnPlayerWin()
        {
            // Show Fireworks celebration
            var fireworksWindow = new FireworksWindow
            {
                Owner = this // Make sure the fireworks window is centered relative to the main game window
            };
            fireworksWindow.Show();
        }

        // Example of a method that detects when the player wins
        private void CheckForWin()
        {
            bool hasPlayerWon = true;  // This would be determined by your game's logic

            if (hasPlayerWon)
            {
                // Call the OnPlayerWin method to trigger the fireworks celebration
                OnPlayerWin();
            }
        }

        // Example method to simulate winning after a button press or other logic
        private void WinButton_Click(object sender, RoutedEventArgs e)
        {
            CheckForWin(); // Simulate a win condition
        }
        // Adding Audio by using mediaplayer
        private void PlaySound(string fileName)
        {
            var mediaPlayer = new MediaPlayer();
            string soundPath = $"pack://siteoforigin:,,,/Sounds/{fileName}";
            mediaPlayer.Open(new Uri(soundPath));
            mediaPlayer.Play();
        }


        // Card class for card data
        public class Card
        {
            public string Value { get; set; }
        }
    }
}
