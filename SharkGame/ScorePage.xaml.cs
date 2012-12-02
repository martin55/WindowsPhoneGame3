namespace SharkGame
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using Microsoft.Phone.Controls;

    using SharkGameLib;

    public partial class ScorePage : PhoneApplicationPage
    {
        /* Fields */

        /// <summary>
        /// High scores collection.
        /// </summary>
        private List<HighScore> highScores;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="ScorePage" /> class.
        /// </summary>
        public ScorePage()
        {
            this.InitializeComponent();

            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            try
            {
                this.highScores = userSettings["High Scores"] as List<HighScore>;
            }
            catch (KeyNotFoundException exception)
            {
                Debug.WriteLine("High scores data not found; " + exception.Message);
                this.highScores = new List<HighScore>();
            }

            if (this.highScores.Count > 0)
            {
                this.highScores = this.highScores.OrderByDescending(x => x.Points).ToList();
            }

            this.highScoresBox.ItemsSource = this.highScores;
        }

        /* Methods */

        /// <summary>
        /// Handles navigating to the Score Page.
        /// </summary>
        /// <param name="e">Information passed to the event.</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string highlight;
            if (NavigationContext.QueryString.TryGetValue("highlight", out highlight))
            {
                int playerScore;
                if (int.TryParse(highlight, out playerScore))
                {
                    this.highlightRow(playerScore);
                }
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Prevent going back to the game when it is over and the high scores are shown
        /// by navigating straight to the Main Menu Page.
        /// </summary>
        /// <param name="e">Information passed to the event.</param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Highlights one row in the high scores so that the player will know which is his score.
        /// </summary>
        /// <param name="playerScore">Index of player's score.</param>
        private void highlightRow(int playerScore)
        {
            if (this.highScoresBox.Items.Count > playerScore)
            {
                this.highScoresBox.SelectedIndex = playerScore;
            }
        }
    }
}