namespace SharkGame
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using Microsoft.Phone.Controls;
    using System;

    public partial class ScorePage : PhoneApplicationPage
    {
        /* Fields */

        /// <summary>
        /// High scores collection.
        /// </summary>
        private Dictionary<string, int> highScores;



        /// <summary>
        /// Initializes a new instance of the <see cref="ScorePage" /> class.
        /// </summary>
        public ScorePage()
        {
            this.InitializeComponent();

            IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;
            try
            {

                this.highScores = userSettings["High scores"] as Dictionary<string, int>;
            }
            catch (KeyNotFoundException exception)
            {
                Debug.WriteLine("High scores data not found; " + exception.Message);
                this.highScores = new Dictionary<string, int>();
            }

            this.highScores.Add("PLAYER", 340);
            this.highScores.Add("PLAYER1", 604);
            this.highScores.Add("PLAYER2", 317);
            this.highScores.Add("PLAYER3", 204);

            this.highScores = this.highScores.OrderByDescending(x => x.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

            this.highScoresBox.ItemsSource = this.highScores;
        }

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
            // TODO: highlight the correct row.
        }
    }
}