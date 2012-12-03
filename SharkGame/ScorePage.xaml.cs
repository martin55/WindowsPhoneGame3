namespace SharkGame
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Media;

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
            // Play the menu song.
            FrameworkDispatcher.Update();
            Song menuSong = (Application.Current as App).Content.Load<Song>("menu");
            if (MediaPlayer.Queue.ActiveSong == null || MediaPlayer.Queue.ActiveSong.Name != menuSong.Name)
            {
                MediaPlayer.Play(menuSong);
                MediaPlayer.IsRepeating = true;
            }

            string highlight;
            if (NavigationContext.QueryString.TryGetValue("highlight", out highlight))
            {
                int playerScore;
                if (int.TryParse(highlight, out playerScore))
                {
                    this.highlightRow(playerScore);
                    NavigationService.RemoveBackEntry();
                }
            }

            base.OnNavigatedTo(e);
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