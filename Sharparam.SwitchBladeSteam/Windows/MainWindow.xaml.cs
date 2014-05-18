using System;
using System.Windows;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;
using Sharparam.SwitchBladeSteam.ViewModels;

using SharpBlade.Razer;

namespace Sharparam.SwitchBladeSteam.Windows
{
    /// <summary>
    /// Interaction logic for SteamFailureWindow.xaml
    /// </summary>
    public partial class MainWindow : ISwitchbladeWindow
    {
        private readonly RazerManager _razer;

        public MainWindow()
        {
            InitializeComponent();

            _razer = Provider.Razer;
            ActivateApp();
            Provider.CurrentWindow = this;

            DataContext = new MainWindowMessageViewModel
            {
                Message = "Unknown initialization error",
                MessageDetails = "Unknown error."
            };

            // Try to init Steam
            try
            {
                var steam = Provider.Steam;
                if (steam.LocalUser == null)
                    return;
                
                Application.Current.MainWindow = new FriendsWindow();
                Close();
                Application.Current.MainWindow.Show();
            }
            catch (SteamException ex)
            {
                var context = new MainWindowMessageViewModel
                {
                    Message = "Steam failed to initialize properly, is Steam running?",
                    MessageDetails = String.Format("Please close app and launch it after Steam has been started.\n\nError detail:\n{0}: {1}", ex.GetType(), ex.Message)
                };
                DataContext = context;
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Helper.ExtendWindowStyleWithTool(this);
        }

        public void ActivateApp()
        {
            _razer.Touchpad.SetWindow(this, Touchpad.RenderMethod.Polling, new TimeSpan(0, 0, 0, 0, 42));
        }

        public void DeactivateApp()
        {
            Application.Current.Shutdown(1);
        }
    }
}
