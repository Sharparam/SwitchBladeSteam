using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Compatibility;

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

            var friend = item as FriendWrapper;

            if (friend == null)
                return;

            Application.Current.MainWindow = new ChatWindow(friend.Friend);
            Close();
            Application.Current.MainWindow.Show();
        }

        private void Friends_OnFilter(object sender, FilterEventArgs e)
        {
            var f = e.Item as FriendWrapper;
            e.Accepted = f != null && f.Friend.Online;
        }
    }
}
