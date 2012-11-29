namespace SharkGame
{
    using System;
    using System.Windows;
    using Microsoft.Phone.Controls;

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