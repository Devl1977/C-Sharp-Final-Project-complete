using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using static MemoryGame.MainWindow;

namespace MemoryGame
{
    public partial class HighScoresWindow : Window
    {
        public HighScoresWindow()
        {
            InitializeComponent();  // Initializes the UI components from XAML
            DisplayHighScores();
        }

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

                // Populate the ListBox
                HighScoresListBox.Items.Clear();
                foreach (var score in topScores)
                {
                    HighScoresListBox.Items.Add($"{score.PlayerName} completed in: {60 - score.TimeTaken} seconds");
                }
            }
            else
            {
                HighScoresListBox.Items.Add("No high scores yet!");
            }
        }
        // Clear High Scores Button Click Event
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Ask for confirmation
            var result = MessageBox.Show("Are you sure you want to clear all high scores?", "Clear High Scores", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                // Clear both ItemsSource and Items to prevent InvalidOperationException
                HighScoresListBox.ItemsSource = null;
                HighScoresListBox.Items.Clear();

                // Set the ItemsSource to an empty list (or leave it null if you don't want to show anything)
                HighScoresListBox.ItemsSource = new List<string>();

                // Clear the saved data
                ClearSavedHighScores();
            }
        }

        // Simulate clearing the saved high scores (e.g., clearing a file)
        private void ClearSavedHighScores()
        {
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MemoryGame");
            string filePath = Path.Combine(directoryPath, "highscores.json");

            // Ensure the file exists before attempting to clear it
            if (File.Exists(filePath))
            {
                // Option 1: Delete the file
                // File.Delete(filePath);

                // Option 2: Overwrite the file with an empty list
                File.WriteAllText(filePath, "[]");
            }
        }
    }
}