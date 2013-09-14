using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.Compatibility
{
    public class MessageWrapper
    {
        public Message Message { get; private set; }

        public string Sender { get; private set; }
        public string Content { get; private set; }

        internal MessageWrapper(Message message)
        {
            Message = message;
            var id = Message.Sender;
            var steam = Provider.Steam;
            Sender = id == steam.LocalUser ? steam.LocalUser.Name : steam.Friends.GetFriendById(id).Name;
            Content = Message.Content;
        }
    }
}
