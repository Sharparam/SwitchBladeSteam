/* FriendsWindow.cs
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sharparam.SwitchBladeSteam.Helpers;
using Sharparam.SwitchBladeSteam.Logging;
using Sharparam.SwitchBladeSteam.Native;
using Sharparam.SwitchBladeSteam.Razer;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam.App
{
    public partial class FriendsWindow : TouchpadForm, IDynamicKeyEnabledForm
    {
        private readonly log4net.ILog _log;

        private readonly List<DynamicKeySettings> _dynamicKeys = new List<DynamicKeySettings>();

        private readonly TimeSpan _updateInterval = new TimeSpan(0, 0, 30);
        private DateTime _lastUpdate;

        private int _selectedIndex;

        public IEnumerable<DynamicKeySettings> DynamicKeys { get { return _dynamicKeys; } } 

        public FriendsWindow(RazerManager manager) : base(manager)
        {
            _log = LogManager.GetLogger(this);
            _log.Debug(">> FriendsWindow()");

            InitializeComponent();

            if (Program.SteamFriends != null)
            {
                UpdateFriendList(true);

                _log.Debug("Subscribing to SteamFriends.FriendsUpdated event");
                Program.SteamFriends.FriendsUpdated += FriendsUpdated;
            }

            _dynamicKeys = new List<DynamicKeySettings>
            {
                new DynamicKeySettings(
                    RazerAPI.DynamicKeyType.DK10,
                    IO.GetAbsolutePath(@"res\images\dk_up.png"),
                    IO.GetAbsolutePath(@"res\images\dk_up_down.png"),
                    (o, e) => MoveSelectionUp()),
                new DynamicKeySettings(
                    RazerAPI.DynamicKeyType.DK5,
                    IO.GetAbsolutePath(@"res\images\dk_down.png"),
                    IO.GetAbsolutePath(@"res\images\dk_down_down.png"),
                    (o, e) => MoveSelectionDown()),
                new DynamicKeySettings(
                    RazerAPI.DynamicKeyType.DK4,
                    IO.GetAbsolutePath(@"res\images\dk_chat.png"),
                    IO.GetAbsolutePath(@"res\images\dk_chat_down.png"),
                    (o, e) => ChatWithSelected())
            };

            _log.Debug("<< FriendsWindow()");
        }

        private void FriendsWindowLoad(object sender, EventArgs e)
        {
#if !DEBUG && RAZER_ENABLED
            WinAPI.EnableScrollBar(FriendList.Handle, WinAPI.SB_BOTH, WinAPI.ESB_DISABLE_BOTH);
            WinAPI.ShowScrollBar(FriendList.Handle, WinAPI.SB_BOTH, false);
#endif
        }

        private void FriendsUpdated(object sender, EventArgs e)
        {
            _log.Debug(">> FriendsUpdated([sender], [e])");
            UpdateFriendList();
            _log.Debug("<< FriendsUpdated()");
        }

        private void UpdateFriendList(bool force = false)
        {
            _log.DebugFormat(">> UpdateFriendList({0})", force ? "true" : "false");

            var now = DateTime.Now;
            if (!force && (now - _lastUpdate) <= _updateInterval)
            {
                _log.Debug("<< UpdateFriendList()");
                return;
            }

            if (InvokeRequired)
            {
                _log.Debug("Invoke is required, calling Invoke method");
                Invoke((VoidDelegate) (() => UpdateFriendList()));
                _log.Debug("<< UpdateFriendList()");
                return;
            }

            _log.Info("Updating the friend list");
            _log.Debug("Clearing old items");
            FriendList.Items.Clear();
            _log.Debug("Retrieving online friends");
            var friends = Program.SteamFriends.Friends.Where(f => f.Online);
            _log.Debug("Creating ImageList to hold avatars");
            var avatars = new ImageList { ImageSize = new Size(96, 96), ColorDepth = ColorDepth.Depth32Bit };
            FriendList.SmallImageList = avatars;
            _log.Debug("Populating friend and avatar list");
            foreach (var friend in friends)
            {
                avatars.Images.Add(friend.GetName(), friend.Avatar);
                FriendList.Items.Add(new ListViewItem(new[] { friend.GetName(), friend.GetStateText() }) { Tag = friend.SteamID, ImageKey = friend.GetName() });
            }
            MoveSelection(0);
            _log.Debug("Friend list updated!");
            _lastUpdate = DateTime.Now;
            _log.Debug("<< UpdateFriendList()");
        }

        private void StartChat(CSteamID id)
        {
            _log.DebugFormat(">> StartChat({0})", id.Render());
            _log.Debug("Queueing the chatwindow form");
            Program.QueueForm(new ChatWindow(Manager, id));
            Close();
            _log.Debug("<< StartChat()");
        }

        private void ChatWithSelected()
        {
            _log.Debug(">> ChatWithSelected()");
            if (FriendList.SelectedItems.Count < 1)
            {
                _log.Debug("Not a valid selection (count < 1), aborting");
                _log.Debug("<< ChatWithSelected()");
                return;
            }

            var item = FriendList.SelectedItems[0];

            if (item == null)
            {
                _log.Debug("Not a valid selection (item is null), aborting");
                _log.Debug("<< ChatWithSelected()");
                return;
            }

            var friendId = (CSteamID) item.Tag;

            _log.DebugFormat("User selected friend: {0}", friendId.Render());

            StartChat(friendId);
            _log.Debug("<< ChatWithSelected()");
        }

#if DEBUG
        public void DebugChatWithSelected()
        {
            ChatWithSelected();
        }
#endif

        private void FriendListDoubleClick(object sender, EventArgs e)
        {
#if DEBUG
            _log.Debug(">> FriendListDoubleClick([sender], [e])");
            ChatWithSelected();
            _log.Debug("<< FriendListDoubleClick()");
#endif
        }

        private void FriendListSelectedIndexChanged(object sender, EventArgs e)
        {
            FriendList.Invalidate(true);
        }

        private void FriendListEnter(object sender, EventArgs e)
        {
            FriendList.Invalidate(true);
        }

        private void FriendsWindowFormClosing(object sender, FormClosingEventArgs e)
        {
            _log.Debug(">> FriendsWindowFormClosing([sender], [e])");
            if (Program.SteamFriends != null)
            {
                _log.Debug("Unsubscribing from SteamFriends.FriendsUpdated event");
                Program.SteamFriends.FriendsUpdated -= FriendsUpdated;
            }
            _log.Debug("<< FriendsWindowFormClosing()");
        }

        private void MoveSelection(int change)
        {
            _log.DebugFormat(">> Move({0})", change);
            try
            {
                if (FriendList.Items.Count <= 0)
                    return;

                int newIndex = _selectedIndex + change;

                if (newIndex < 0)
                    newIndex = 0;
                else if (newIndex >= FriendList.Items.Count)
                    newIndex = FriendList.Items.Count - 1;

                var oldItem = FriendList.Items[_selectedIndex];
                if (oldItem != null)
                {
                    oldItem.Focused = false;
                    oldItem.Selected = false;
                }

                var newItem = FriendList.Items[newIndex];
                if (newItem != null)
                {
                    newItem.Selected = true;
                    newItem.Focused = true;
                }

                FriendList.Select();
                FriendList.Focus();
                FriendList.EnsureVisible(newIndex);

                _selectedIndex = newIndex;

                FriendList.Invalidate(true);
            }
            catch (ObjectDisposedException)
            {
                _log.Warn("Move called when FriendList was disposed, aborting");
            }
            _log.Debug("<< Move()");
        }

        public void MoveSelectionUp()
        {
            MoveSelection(-1);
        }

        public void MoveSelectionDown()
        {
            MoveSelection(1);
        }
    }
}
