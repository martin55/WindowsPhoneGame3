namespace SharkGame
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents a page for main game menu, providing access to options as well as starting the game itself.
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /* Constructor */

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        /* Methods */

        /// <summary>
        /// Handles navigating to the Main Page.
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

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Starts the game itself.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Opens options menu.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void ShowOptions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/OptionsPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Opens High scores menu.
        /// </summary>
        /// <param name="sender">Object that raised the event.</param>
        /// <param name="e">Information passed to the event.</param>
        private void ShowScores_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ScorePage.xaml", UriKind.Relative));
        }
    }
}