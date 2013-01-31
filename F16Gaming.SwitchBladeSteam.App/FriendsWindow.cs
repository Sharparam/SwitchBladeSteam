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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using F16Gaming.SwitchBladeSteam.Logging;
using F16Gaming.SwitchBladeSteam.Razer;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.App
{
	public partial class FriendsWindow : Form
	{
		private readonly log4net.ILog _log;

		private readonly TimeSpan _updateInterval = new TimeSpan(0, 0, 30);
		private DateTime _lastUpdate;

		public FriendsWindow()
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

			_log.Debug("<< FriendsWindow()");
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
			var avatars = new ImageList { ImageSize = new Size(64, 64), ColorDepth = ColorDepth.Depth32Bit };
			FriendList.SmallImageList = avatars;
			_log.Debug("Populating friend and avatar list");
			foreach (var friend in friends)
			{
				avatars.Images.Add(friend.GetName(), friend.Avatar);
				FriendList.Items.Add(new ListViewItem(new[] { friend.GetName(), friend.GetStateText() }) { Tag = friend.SteamID, ImageKey = friend.GetName() });
			}
			_log.Debug("Friend list updated!");
			_lastUpdate = DateTime.Now;
			_log.Debug("<< UpdateFriendList()");
		}

		private void FriendListSelectedIndexChanged(object sender, EventArgs e)
		{
			_log.Debug(">> FriendListSelectedIndexChanged([sender], [e])");
			if (FriendList.SelectedItems.Count < 1)
			{
				_log.Debug("Not a valid selection (count < 1), aborting");
				_log.Debug("<< FriendListSelectedIndexChanged()");
				return;
			}

			var item = FriendList.SelectedItems[0];

			if (item == null)
			{
				_log.Debug("Not a valid selection (item is null), aborting");
				_log.Debug("<< FriendListSelectedIndexChanged()");
				return;
			}

			var friendId = (CSteamID) item.Tag;

			_log.DebugFormat("User selected friend: {0}", friendId.Render());

			_log.Debug("Queueing the chatwindow form");
			Program.QueueForm(new ChatWindow(friendId));
			Close();
			_log.Debug("<< FriendListSelectedIndexChanged()");
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
	}
}
