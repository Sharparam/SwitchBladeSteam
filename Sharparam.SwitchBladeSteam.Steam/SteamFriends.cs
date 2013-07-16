/* SteamFriends.cs
 *
 * Copyright © 2013 by Adam Hellberg
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
 * of the Software, and to permit persons to whom the Software is furnished to do
 * so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * Disclaimer: SwitchBladeSteam is in no way affiliated
 * with Razer and/or any of its employees and/or licensors.
 * Adam Hellberg does not take responsibility for any harm caused, direct
 * or indirect, to any Razer peripherals via the use of SwitchBladeSteam.
 * 
 * "Razer" is a trademark of Razer USA Ltd.
 */

using System.Drawing;
using System.Text;
using Sharparam.SharpBlade.Logging;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam.Steam
{
    public class SteamFriends
    {
        private readonly ILog _log;

        private readonly ISteamFriends002 _steamFriends002; // Chat
        private readonly ISteamFriends013 _steamFriends013; // Avatars
        private readonly IClientFriends _clientFriends; // The only one with nickname support

        private readonly ISteamUtils005 _steamUtils; // Avatar utils

        internal ISteamFriends002 SteamFriends002 { get { return _steamFriends002; } }
        internal ISteamFriends013 SteamFriends013 { get { return _steamFriends013; } }
        internal IClientFriends ClientFriends { get { return _clientFriends; } }

        internal SteamFriends(
            ISteamFriends002 steamFriends002,
            ISteamFriends013 steamFriends013,
            IClientFriends clientFriends,
            ISteamUtils005 steamUtils)
        {
            _log = LogManager.GetLogger(this);	

            _steamFriends002 = steamFriends002;
            _steamFriends013 = steamFriends013;
            _clientFriends = clientFriends;
            _steamUtils = steamUtils;
        }

        public string GetMyName()
        {
            return _steamFriends002.GetPersonaName();
        }

        public EPersonaState GetMyState()
        {
            return _steamFriends002.GetPersonaState();
        }

        public string GetMyStateText()
        {
            return Utils.StateToString(GetMyState());
        }

        public void SetMyState(EPersonaState state)
        {
            if (GetMyState() == state)
                return;

            _steamFriends002.SetPersonaState(state);
        }

        public int GetFriendCount(EFriendFlags flags)
        {
            return _steamFriends002.GetFriendCount(flags);
        }

        public CSteamID GetFriendByIndex(int index, EFriendFlags flags)
        {
            return _steamFriends002.GetFriendByIndex(index, flags);
        }

        public string GetFriendName(CSteamID id)
        {
            return _steamFriends002.GetFriendPersonaName(id);
        }

        public string GetFriendNickname(CSteamID id)
        {
            return _clientFriends.GetPlayerNickname(id);
        }

        public EPersonaState GetFriendState(CSteamID id)
        {
            return _steamFriends002.GetFriendPersonaState(id);
        }

        public string GetFriendStateText(CSteamID id)
        {
            return Utils.StateToString(GetFriendState(id));
        }

        public Bitmap GetFriendAvatar(CSteamID id, EAvatarSize size)
        {
            int handle;
            switch (size)
            {
                case EAvatarSize.k_EAvatarSize32x32:
                    handle = _steamFriends013.GetSmallFriendAvatar(id);
                    break;
                case EAvatarSize.k_EAvatarSize64x64:
                    handle = _steamFriends013.GetMediumFriendAvatar(id);
                    break;
                case EAvatarSize.k_EAvatarSize184x184:
                    handle = _steamFriends013.GetLargeFriendAvatar(id);
                    break;
                default:
                    handle = _steamFriends013.GetLargeFriendAvatar(id);
                    break;
            }
            var avatar = Utils.GetAvatarFromHandle(handle, _steamUtils);
            return avatar;
        }

        public Bitmap GetSmallFriendAvatar(CSteamID id)
        {
            return GetFriendAvatar(id, EAvatarSize.k_EAvatarSize32x32);
        }

        public Bitmap GetMediumFriendAvatar(CSteamID id)
        {
            return GetFriendAvatar(id, EAvatarSize.k_EAvatarSize64x64);
        }

        public Bitmap GetLargeFriendAvatar(CSteamID id)
        {
            return GetFriendAvatar(id, EAvatarSize.k_EAvatarSize184x184);
        }

        public ChatMessage GetChatMessage(CSteamID sender, CSteamID receiver, int chatId)
        {
            var data = new byte[4096];
            var type = EChatEntryType.k_EChatEntryTypeChatMsg;
            var length = _steamFriends002.GetChatMessage(receiver, chatId, data, ref type);
            var msg = Encoding.UTF8.GetString(data, 0, length - 1);
            var message = new ChatMessage(sender, receiver, type, msg);
            return message;
        }

        public ChatMessage GetChatMessage(CSteamID sender, CSteamID receiver, uint chatId)
        {
            return GetChatMessage(sender, receiver, (int) chatId);
        }

        public ChatMessage GetChatMessage(FriendChatMsg_t callback)
        {
            var sender = new CSteamID(callback.m_ulSenderID);
            var receiver = new CSteamID(callback.m_ulFriendID);
            var id = callback.m_iChatID;
            return GetChatMessage(sender, receiver, id);
        }

        public void SendMessage(CSteamID receiver, EChatEntryType type, string message)
        {
            if (type == EChatEntryType.k_EChatEntryTypeEmote)
                _log.Warn("Steam no longer supports sending emotes to chat");
            _steamFriends002.SendMsgToFriend(receiver, type, Encoding.UTF8.GetBytes(message));
        }

        public void SendChatMessage(CSteamID receiver, string message)
        {
            SendMessage(receiver, EChatEntryType.k_EChatEntryTypeChatMsg, message);
        }
    }
}
