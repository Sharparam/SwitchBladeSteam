﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer;
using Sharparam.SharpBlade.Razer.Events;
using Sharparam.SteamLib;
using Sharparam.SteamLib.Events;
using Sharparam.SwitchBladeSteam.Compatibility;
using Sharparam.SwitchBladeSteam.Lib;
using Sharparam.SwitchBladeSteam.ViewModels;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam.Windows
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        private const string DefaultInputMessage = "Tap screen to type a message...";
        private const string TitleFormat = "{0} ({1})";

        private static readonly SolidColorBrush IdleColor = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        private static readonly SolidColorBrush ActiveColor = new SolidColorBrush(Color.FromArgb(100, 51, 153, 0));

        private readonly RazerManager _razer;

        private readonly Friend _friend;

        private ScrollViewer _historyBoxScroller;

        public ChatWindow(Friend friend)
        {
            InitializeComponent();

            _friend = friend;

            TitleLabel.Content = String.Format(TitleFormat, _friend.Name, _friend.Nickname);

            _friend.TypingMessageReceived += FriendOnTypingMessageReceived;
            _friend.ChatMessageReceived += FriendOnChatMessageReceived;

            var viewModel = FriendViewModel.GetViewModel(_friend);
            DataContext = viewModel;
            ((INotifyCollectionChanged) viewModel.Messages.Messages).CollectionChanged += MessagesCollectionChanged;

            _razer = Provider.Razer;
            _razer.Touchpad.SetWindow(this, Touchpad.RenderMethod.Polling, new TimeSpan(0, 0, 0, 0, 42));
            _razer.Touchpad.EnableGesture(RazerAPI.GestureType.Tap);
            _razer.Touchpad.Gesture += TouchpadOnGesture;

            // Set up dynamic keys
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK1, FriendsKeyPressed, @"Resources\Images\dk_friends.png",
                                    @"Resources\Images\dk_friends_down.png", true);
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK10, (s, e) => ScrollHistoryBoxUp(),
                                    @"Resources\Images\dk_up.png", @"Resources\Images\dk_up_down.png", true);
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK5, (s, e) => ScrollHistoryBoxDown(),
                                    @"Resources\Images\dk_down.png", @"Resources\Images\dk_down_down.png", true);
            _razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK4, (s, e) => ScrollHistoryBoxToEnd(),
                                    @"Resources\Images\dk_bottom.png", @"Resources\Images\dk_bottom_down.png", true);
        }

        private void SetTitle(string content)
        {
            if (TitleLabel.Dispatcher.CheckAccess())
                TitleLabel.Content = content;
            else
                TitleLabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (() => SetTitle(content)));
        }

        private void ScrollHistoryBoxToEnd()
        {
            try
            {
                if (_historyBoxScroller.Dispatcher.CheckAccess())
                    _historyBoxScroller.ScrollToEnd();
                else
                    _historyBoxScroller.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) (ScrollHistoryBoxToEnd));
            }
            catch (NullReferenceException)
            {

            }
        }

        private void ScrollHistoryBoxUp()
        {
            try
            {
                if (_historyBoxScroller.Dispatcher.CheckAccess())
                    _historyBoxScroller.LineUp();
                else
                    _historyBoxScroller.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(ScrollHistoryBoxUp));
            }
            catch (NullReferenceException)
            {

            }
        }

        private void ScrollHistoryBoxDown()
        {
            try
            {
                if (_historyBoxScroller.Dispatcher.CheckAccess())
                    _historyBoxScroller.LineDown();
                else
                    _historyBoxScroller.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(ScrollHistoryBoxDown));
            }
            catch (NullReferenceException)
            {

            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            Helper.ExtendWindowStyleWithTool(this);
            _historyBoxScroller = Helper.GetDescendantByType<ScrollViewer>(HistoryBox);
            if (HistoryBox.SelectedIndex > -1)
                HistoryBox.SelectedIndex = -1;
        }

        private void FriendOnTypingMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            if (TitleLabel.Dispatcher.CheckAccess())
                SetTitle(TitleLabel.Content + " is typing...");
            else
                TitleLabel.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                  (Action) (() => SetTitle(TitleLabel.Content + " is typing...")));
        }

        private void FriendOnChatMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            SetTitle(String.Format(TitleFormat, _friend.Name, _friend.Nickname));
        }

        private void MessagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            ScrollHistoryBoxToEnd();
        }

        private void FriendsKeyPressed(object sender, EventArgs eventArgs)
        {
            _razer.Touchpad.DisableGesture(RazerAPI.GestureType.Tap);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK1);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK10);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK5);
            _razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK4);
            Application.Current.MainWindow = new FriendsWindow();
            Close();
            Application.Current.MainWindow.Show();
        }

        private void TouchpadOnGesture(object sender, GestureEventArgs args)
        {
            if (args.GestureType != RazerAPI.GestureType.Tap)
                return;
            
            InputBox.Clear();
            InputBox.Background = ActiveColor;
            _razer.StartWPFControlKeyboardCapture(InputBox, false);
        }

        private void InputBoxKeyDown(object sender, KeyEventArgs e)
        {
            var msg = InputBox.Text;

            if (!String.IsNullOrEmpty(msg))
            {
                msg = msg.Replace("  ", " ");
                InputBox.Text = msg;
                InputBox.CaretIndex = InputBox.Text.Length;
                var rect = InputBox.GetRectFromCharacterIndex(InputBox.CaretIndex);
                InputBox.ScrollToHorizontalOffset(rect.Right);
            }

            if (e.Key != Key.Return)
            {
                _friend.SendMessage(msg, EChatEntryType.k_EChatEntryTypeTyping);
                return;
            }

            if (!String.IsNullOrEmpty(msg))
            {
                _friend.SendMessage(msg);
                InputBox.CaretIndex = InputBox.Text.Length;
                var rect = InputBox.GetRectFromCharacterIndex(InputBox.CaretIndex);
                InputBox.ScrollToHorizontalOffset(rect.Right);
            }

            InputBox.Background = IdleColor;
            _razer.SetKeyboardCapture(false);
            InputBox.Text = DefaultInputMessage;
        }
    }
}
