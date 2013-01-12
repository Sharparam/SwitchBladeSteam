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
using F16Gaming.SwitchBladeSteam.Razer;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.App
{
	public partial class FriendsWindow : Form
	{
		public FriendsWindow()
		{
			InitializeComponent();

			UpdateFriendList();

			Program.SteamFriends.FriendsUpdated += FriendsUpdated;
		}

		private void FriendsUpdated(object sender, EventArgs e)
		{
			UpdateFriendList();
		}

		private void UpdateFriendList()
		{
			if (InvokeRequired)
			{
				Invoke((VoidDelegate) UpdateFriendList);
				return;
			}

			var friends = Program.SteamFriends.Friends.Where(f => f.Online);
			var avatars = new ImageList { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };
			FriendList.SmallImageList = avatars;
			foreach (var friend in friends)
			{
				avatars.Images.Add(friend.GetName(), friend.Avatar);
				FriendList.Items.Add(new ListViewItem(new[] { friend.GetName(), friend.GetStateText() }) { Tag = friend.SteamID, ImageKey = friend.GetName() });
			}
		}

		private void FriendListSelectedIndexChanged(object sender, EventArgs e)
		{
			if (FriendList.SelectedItems.Count < 1)
				return;

			var item = FriendList.SelectedItems[0];

			if (item == null)
				return;

			var friendId = (CSteamID) item.Tag;

			Program.QueueForm(new ChatWindow(friendId));
			Close();
		}

		private void FriendsWindowFormClosing(object sender, FormClosingEventArgs e)
		{
			Program.SteamFriends.FriendsUpdated -= FriendsUpdated;
		}
	}
}
