﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Globalization;

namespace Bluegrams.Application.WPF
{
    /// <summary>
    /// Provides a basic 'About' box showing important app information.
    /// </summary>
    public partial class AboutBox : Window
    {
        /// <summary>
        /// Specifies the service used for update checking when clicking the update button.
        /// </summary>
        public IUpdateChecker UpdateChecker { get; set; }

        /// <summary>
        /// A custom color used e.g. for coloring the heading of the window.
        /// </summary>
        public Color HighlightColor { get; set; } = Colors.DarkGray;

        /// <summary>
        /// Creates a new instance of the class AboutBox.
        /// </summary>
        /// <param name="icon">Product icon.</param>
        public AboutBox(ImageSource icon = null)
        {
            this.DataContext = this;
            InitializeComponent();
            if (icon != null)
            {
                this.imgIcon.Source = icon;
                this.Icon = icon;
                brdIcon.Background = new SolidColorBrush(Colors.Transparent);
            }
            this.Title = Properties.Resources.strAbout + " " + Title;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppInfo.SupportedCultures?.Length > 0)
            {
                foreach (CultureInfo cu in AppInfo.SupportedCultures)
                {
                    comLanguages.Items.Add(cu.DisplayName);
                    if (cu.TwoLetterISOLanguageName == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                        comLanguages.SelectedIndex = comLanguages.Items.Count - 1;
                }
            }
            else
            {
                stackLang.Visibility = Visibility.Collapsed;
            }
            if (UpdateChecker != null)
                butUpdate.Visibility = Visibility.Visible;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Uri.OriginalString))
            {
                try
                {
                    Process.Start(e.Uri.OriginalString);
                }
                catch { /* Silently fail */ }
            }
        }

        private void butRestart_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.InfoWindow_RestartNewLang, "", MessageBoxButton.OKCancel, MessageBoxImage.Warning) 
                == MessageBoxResult.OK)
            {
                changeCulture(AppInfo.SupportedCultures[comLanguages.SelectedIndex]);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void butUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateChecker.CheckForUpdates(UpdateNotifyMode.Always);
        }

        // Note: Needs WindowManager instance for automatic change.
        private void changeCulture(CultureInfo culture)
        {
            Properties.Settings.Default.Culture = culture.Name;
            Properties.Settings.Default.Save();
            System.Windows.Application.Current.Shutdown();
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string[] args = new string[Environment.GetCommandLineArgs().Length - 1];
                Array.Copy(Environment.GetCommandLineArgs(), 1, args, 0, args.Length);
                Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location, String.Join(" ", args));
            }
            else Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
