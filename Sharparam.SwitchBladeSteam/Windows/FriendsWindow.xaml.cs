using System.Linq;
using System.Windows;
using System.Windows.Input;
using Sharparam.SwitchBladeSteam.Lib;

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

            foreach (var friend in Provider.Steam.FriendsManager.Friends.Where(f => f.Online).OrderBy(f => f.GetName()))
                FriendsListBox.Items.Add(friend.GetName());
        }

        private void FriendsListBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != Key.Return || (e.Key == Key.Enter && FriendsListBox.SelectedItem == null))
                return;

            var friend = Provider.Steam.FriendsManager.GetFriendByMatching((string) FriendsListBox.SelectedItem, false);
            Application.Current.MainWindow = new ChatWindow(friend);
            Close();
            Application.Current.MainWindow.Show();
        }
    }
}
