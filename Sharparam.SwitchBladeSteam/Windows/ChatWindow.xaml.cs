using System;
using System.Windows.Input;
using System.Windows.Threading;
using Sharparam.SteamLib;
using Sharparam.SteamLib.Events;
using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.Windows
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        private delegate void VoidDelegate();

        private const string TitleFormat = "Chatting with {0}";
        private const string MessageFormat = "{0}: {1}";

        private Friend _friend;

        public ChatWindow(Friend friend)
        {
            InitializeComponent();

            _friend = friend;

            TitleLabel.Content = String.Format(TitleFormat, _friend.GetName());

            if (_friend.ChatHistory.Count > 0)
                foreach (var message in _friend.ChatHistory)
                {
                    HistoryBox.Items.Add(String.Format(
                        MessageFormat,
                        Provider.Steam.FriendsManager.GetFriendBySteamId(message.Sender).GetName(),
                        Provider.Steam.FriendsManager.GetFriendBySteamId(message.Receiver).GetName()));
                }

            _friend.ChatMessageReceived += FriendOnChatMessageReceived;
        }

        private void FriendOnChatMessageReceived(object sender, ChatMessageEventArgs args)
        {
            var message = args.Message;
            HistoryBox.Dispatcher.Invoke(DispatcherPriority.Send, (VoidDelegate) (() => HistoryBox.Items.Add(String.Format(
                MessageFormat,
                message.Sender == Provider.Steam.Me
                    ? Provider.Steam.GetMyName()
                    : Provider.Steam.FriendsManager.GetFriendBySteamId(message.Sender).GetName(),
                message.Message.Replace('\n', ' ')))));
        }

        private void InputBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return || (e.Key == Key.Return && String.IsNullOrEmpty(InputBox.Text)))
                return;

            _friend.SendMessage(InputBox.Text);
            InputBox.Text = "";
        }
    }
}
