using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer;
using Sharparam.SwitchBladeSteam.Compatibility;
using Sharparam.SwitchBladeSteam.Lib;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam.Windows
{
    /// <summary>
    /// Interaction logic for FriendsWindow.xaml
    /// </summary>
    public partial class FriendsWindow
    {
        private RazerManager _razer;

        public FriendsWindow()
        {
            InitializeComponent();

            _razer = Provider.Razer;
            _razer.Touchpad.DisableOSGesture(RazerAPI.GestureType.All);
            _razer.Touchpad.SetWindow(this, Touchpad.RenderMethod.Polling, new TimeSpan(0, 0, 0, 0, 42));

            // Set up dynamic keys
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK1, (s, e) => StartChatWithSelected(),
                                    @"Resources\Images\dk_chat.png", @"Resources\Images\dk_chat_down.png", true);
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK10, (s, e) => MoveSelectionUp(),
                                    @"Resources\Images\dk_up.png", @"Resources\Images\dk_up_down.png", true);
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK5, (s, e) => MoveSelectionDown(),
                                    @"Resources\Images\dk_down.png", @"Resources\Images\dk_down_down.png", true);

            // These keys are only enabled once
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK6,
                                    (s, e) => { Provider.Steam.LocalUser.State = EPersonaState.k_EPersonaStateOnline; },
                                    @"Resources\Images\dk_appear_online.png",
                                    @"Resources\Images\dk_appear_online_down.png");
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK9,
                                    (s, e) => { Provider.Steam.LocalUser.State = EPersonaState.k_EPersonaStateOffline; },
                                    @"Resources\Images\dk_appear_offline.png",
                                    @"Resources\Images\dk_appear_offline_down.png");
        }

        private void FriendsListBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && (e.Key != Key.Enter || FriendsListBox.SelectedItem != null))
                StartChatWithSelected();
        }

        private void StartChatWithSelected()
        {
            var item = FriendsListBox.SelectedItem;

            var friend = item as FriendWrapper;

            if (friend == null)
                return;

            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK1);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK10);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK5);

            Application.Current.MainWindow = new ChatWindow(friend.Friend);
            Close();
            Application.Current.MainWindow.Show();
        }

        private void MoveSelection(int direction)
        {
            if (FriendsListBox.Items.Count <= 1 ||
                FriendsListBox.SelectedIndex == -1 ||
                (FriendsListBox.SelectedIndex == 0 && direction == -1) ||
                (FriendsListBox.SelectedIndex == FriendsListBox.Items.Count - 1 && direction == 1))
                return;

            FriendsListBox.SelectedIndex += direction;
            FriendsListBox.ScrollIntoView(FriendsListBox.SelectedItem);
        }

        private void MoveSelectionUp()
        {
            MoveSelection(-1);
        }

        private void MoveSelectionDown()
        {
            MoveSelection(1);
        }

        private void Friends_OnFilter(object sender, FilterEventArgs e)
        {
            var f = e.Item as FriendWrapper;
            e.Accepted = f != null && f.Friend.Online;
        }
    }
}
