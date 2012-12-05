namespace SharkGame
{
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Represents a page containing all game options, accessible from the main game menu.
    /// </summary>
    public partial class OptionsPage : PhoneApplicationPage
    {
        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPage" /> class.
        /// </summary>
        public OptionsPage()
        {
            this.InitializeComponent();
        }

        /* Methods */

        /// <summary>
        /// Handles moving the music volume slider.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void MusicSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO: Adjust IsolatedStorageSettings
        }

        /// <summary>
        /// Handles moving the gore level slider.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void GoreSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO: Adjust IsolatedStorageSettings
        }

        /// <summary>
        /// Handles clicking the clear high scores button.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Detailed state connected with the event.</param>
        private void ClearHighScores_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Adjust IsolatedStorageSettings
        }
    }
}