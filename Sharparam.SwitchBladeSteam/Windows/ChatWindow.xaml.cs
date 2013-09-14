using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Sharparam.SteamLib;
using Sharparam.SteamLib.Events;
using Sharparam.SwitchBladeSteam.Compatibility;
using Sharparam.SwitchBladeSteam.Lib;
using Sharparam.SwitchBladeSteam.ViewModels;

namespace Sharparam.SwitchBladeSteam.Windows
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        private delegate void VoidDelegate();

        private const string TitleFormat = "Chatting with {0} ({1})";
        private const string MessageFormat = "{0}: {1}";

        private readonly Friend _friend;

        public ChatWindow(Friend friend)
        {
            InitializeComponent();

            _friend = friend;

            TitleLabel.Content = String.Format(TitleFormat, _friend.Name, _friend.Nickname);

            if (_friend.ChatMessageHistory.Any())
                foreach (var message in _friend.ChatMessageHistory)
                {
                    HistoryBox.Items.Add(String.Format(
                        MessageFormat,
                        Provider.Steam.Friends.GetFriendById(message.Sender).Name,
                        message.Content));
                }

            _friend.ChatMessage += FriendOnChatMessage;
        }

        private void FriendOnChatMessage(object sender, MessageEventArgs args)
        {
            var message = args.Message;
            HistoryBox.Dispatcher.Invoke(DispatcherPriority.Send, (VoidDelegate)(() => HistoryBox.Items.Add(String.Format(
                MessageFormat,
                message.Sender == Provider.Steam.LocalUser
                    ? Provider.Steam.LocalUser.Name
                    : Provider.Steam.Friends.GetFriendById(message.Sender).Name,
                message.Content.Replace('\n', ' ')))));
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
