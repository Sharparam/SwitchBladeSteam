using System.Windows;
using System.Windows.Input;
using Sharparam.SteamLib;

namespace Sharparam.SwitchBladeSteam.Windows
{
    /// <summary>
    /// Interaction logic for FriendsWindow.xaml
    /// </summary>
    public partial class FriendsWindow
    {
        public FriendsWindow()
        {
            InitializeComponent();
        }

        private void FriendsListBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return || (e.Key == Key.Enter && FriendsListBox.SelectedItem == null))
                return;

            var item = FriendsListBox.SelectedItem;

            var friend = item as Friend;

            if (friend == null)
                return;

            Application.Current.MainWindow = new ChatWindow(friend);
            Close();
            Application.Current.MainWindow.Show();
        }
    }
}
