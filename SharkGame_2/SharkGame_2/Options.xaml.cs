using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace SharkGame_2
{

    

    public partial class Options : PhoneApplicationPage
    {
        bool sfx = true;
        bool music = true;


        public Options()
        {
            InitializeComponent();
        }

       

        
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/GamePage.xaml?msg=" + 
                checkBox1.IsChecked + checkBox2.IsChecked, UriKind.Relative));
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox1_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {

        }

      
       

       
    }
}