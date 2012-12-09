namespace SharkGame
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO.IsolatedStorage;
    using Microsoft.Phone.Controls;
    using Microsoft.Xna.Framework.Media;
    using SharkGameLib;

    /// <summary>
    /// Represents a page containing all game options, accessible from the main game menu.
    /// </summary>
    public partial class OptionsPage : PhoneApplicationPage
    {
        /* Fields */
        IsolatedStorageSettings userSettings = IsolatedStorageSettings.ApplicationSettings;

        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPage" /> class.
        /// </summary>
        public OptionsPage()
        {
            this.InitializeComponent();

            double? soundVolume;
            try
            {
                soundVolume = this.userSettings["Sound Volume"] as double?;
            }
            catch (KeyNotFoundException exception)
            {
                Debug.WriteLine("Sound Volume variable not found; " + exception.Message);
                soundVolume = 66.6;
            }

            this.MusicSlider.Value = soundVolume.Value;
            this.GoreSlider.Value = 100.0;
        }

        /* Methods */

        /// <summary>
        /// Handles moving the music volume slider.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void MusicSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            MediaPlayer.Volume = (float) (e.NewValue / 100.0);
        }

        /// <summary>
        /// Handles moving the gore level slider.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void GoreSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (userSettings.Contains("Gore Level"))
            {
                userSettings["Gore Level"] = e.NewValue;
            }
            else
            {
                userSettings.Add("Gore Level", e.NewValue);
            }

            // TODO: Implement actual in-game blood and gore level.

            userSettings.Save();
        }

        /// <summary>
        /// Handles clicking the clear high scores button.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void ClearHighScores_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (userSettings.Contains("High Scores"))
            {
                userSettings["High Scores"] = new List<HighScore>();
            }
            else
            {
                userSettings.Add("High Scores", new List<HighScore>());
            }

            userSettings.Save();
        }
    }
}