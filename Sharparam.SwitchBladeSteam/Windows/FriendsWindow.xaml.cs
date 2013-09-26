using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer;
using Sharparam.SharpBlade.Razer.Events;
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
        private const int ScrollThreshold = 15;

        private readonly RazerManager _razer;

        private int _pressYPos;
        private int _lastYPos;
        private int _deltaYPos;

        public FriendsWindow()
        {
            InitializeComponent();

            _razer = Provider.Razer;
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

            _razer.Touchpad.Gesture += TouchpadOnGesture;
            _razer.Touchpad.EnableGesture(RazerAPI.GestureType.Press);
            _razer.Touchpad.EnableGesture(RazerAPI.GestureType.Move);
        }

        private void TouchpadOnGesture(object sender, GestureEventArgs gestureEventArgs)
        {
            switch (gestureEventArgs.GestureType)
            {
                case RazerAPI.GestureType.Press:
                    _pressYPos = gestureEventArgs.Y;
                    _lastYPos = _pressYPos;
                    _deltaYPos = 0;
                    break;
                case RazerAPI.GestureType.Move:
                    var yPos = gestureEventArgs.Y;
                    var change = yPos - _lastYPos;
                    _deltaYPos += change;
                    if (Math.Abs(_deltaYPos) > ScrollThreshold)
                    {
                        MoveSelection(-_deltaYPos);
                        _deltaYPos = 0;
                    }
                    _lastYPos = yPos;
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Helper.ExtendWindowStyleWithTool(this);
        }

        private void StartChatWithSelected()
        {
            var item = FriendsListBox.SelectedItem;

            var friend = item as FriendWrapper;

            if (friend == null)
                return;

            _razer.Touchpad.DisableGesture(RazerAPI.GestureType.Press);
            _razer.Touchpad.DisableGesture(RazerAPI.GestureType.Move);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK1);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK10);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK5);
            _razer.Touchpad.Gesture -= TouchpadOnGesture;

            Application.Current.MainWindow = new ChatWindow(friend.Friend);
            Close();
            Application.Current.MainWindow.Show();
        }

        private void MoveSelection(int direction)
        {
            if (FriendsListBox.Items.Count <= 1 ||
                FriendsListBox.SelectedIndex == -1 ||
                (FriendsListBox.SelectedIndex == 0 && direction < 0) ||
                (FriendsListBox.SelectedIndex == FriendsListBox.Items.Count - 1 && direction > 0))
                return;

            FriendsListBox.SelectedIndex += (direction == 0 ? 0 : (direction < 0 ? -1 : 1));
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
