﻿#pragma checksum "C:\Users\Martin\Documents\Visual Studio 2010\Projects\WindowsPhoneGame3\SharkGame_2\SharkGame_2\Options.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "047568F3DF2E866658344CEDAA0520F0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SharkGame_2 {
    
    
    public partial class Options : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.CheckBox checkBox1;
        
        internal System.Windows.Controls.RadioButton radioButton1;
        
        internal System.Windows.Controls.Slider slider1;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SharkGame_2;component/Options.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.checkBox1 = ((System.Windows.Controls.CheckBox)(this.FindName("checkBox1")));
            this.radioButton1 = ((System.Windows.Controls.RadioButton)(this.FindName("radioButton1")));
            this.slider1 = ((System.Windows.Controls.Slider)(this.FindName("slider1")));
        }
    }
}

