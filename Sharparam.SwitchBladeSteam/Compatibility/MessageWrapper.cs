using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class MessageWrapper
    {
        public Message Message { get; private set; }

        public string Sender { get; private set; }
        public string Content { get; private set; }

        public bool IsLocalUser { get; private set; }
        public bool IsInGame { get; private set; }

        public SolidColorBrush MessageColor
        {
            get
            {
                return IsLocalUser
                           ? new SolidColorBrush(Color.FromRgb(138, 138, 138))
                           : new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        internal MessageWrapper(Message message)
        {
            Message = message;
            var id = Message.Sender;
            var steam = Provider.Steam;
            IsLocalUser = id == steam.LocalUser;
            Sender = IsLocalUser ? steam.LocalUser.Name : steam.Friends.GetFriendById(id).Name;
            Content = Message.Content;
            IsInGame = IsLocalUser ? steam.LocalUser.InGame : steam.Friends.GetFriendById(id).InGame;
        }
    }
}
