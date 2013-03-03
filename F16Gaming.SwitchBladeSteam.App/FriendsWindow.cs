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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using F16Gaming.SwitchBladeSteam.Logging;
using F16Gaming.SwitchBladeSteam.Native;
using F16Gaming.SwitchBladeSteam.Razer;
using F16Gaming.SwitchBladeSteam.Razer.Events;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.App
{
	public partial class FriendsWindow : Form, IGestureEnabledForm
	{
		private readonly log4net.ILog _log;

		private readonly TimeSpan _updateInterval = new TimeSpan(0, 0, 30);
		private DateTime _lastUpdate;

		private bool _handlingGesture;
		private int _lastY = -1;
		private int _yChange;
		private int _lastPressParam;
		private int _lastPressStartY;

		private readonly int _yScrollDetectAmount;

		public RazerAPI.RZGESTURE EnabledGestures { get { return RazerAPI.RZGESTURE.PRESS; } }

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

			// TEMPORARY TESTING VALUE FILE
			if (File.Exists("scroll_value"))
			{
				_yScrollDetectAmount = int.Parse(File.ReadAllText("scroll_value"));
			}
			else
			{
				_yScrollDetectAmount = 15;
				File.WriteAllText("scroll_value", _yScrollDetectAmount.ToString(CultureInfo.InvariantCulture));
			}

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
			_log.Debug("Friend list updated!");
			_lastUpdate = DateTime.Now;
			_log.Debug("<< UpdateFriendList()");
		}

		private void StartChat(CSteamID id)
		{
			_log.DebugFormat(">> StartChat({0})", id.Render());
			_log.Debug("Queueing the chatwindow form");
			Program.QueueForm(new ChatWindow(id));
			Close();
			_log.Debug("<< StartChat()");
		}

#if DEBUG
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
#endif

		private ListViewItem GetListItemUnderCursor()
		{
			_log.Debug(">> GetListItemUnderCursor()");
			var cursorPos = Cursor.Position;
			_log.DebugFormat("Cursor X,Y == {0},{1}", cursorPos.X, cursorPos.Y);
			Point localPoint = FriendList.PointToClient(cursorPos);
			_log.DebugFormat("localPoint X,Y == {0},{1}", localPoint.X, localPoint.Y);
			_log.Debug("<< GetListItemUnderCursor()");
			return FriendList.GetItemAt(localPoint.X, localPoint.Y);
		}

		private void FriendListDoubleClick(object sender, EventArgs e)
		{
#if DEBUG
			_log.Debug(">> FriendListDoubleClick([sender], [e])");
			ChatWithSelected();
			_log.Debug("<< FriendListDoubleClick()");
#endif
		}

		private void FriendListScroll(object sender, EventArgs e)
		{
			_log.Debug(">> FriendListScroll([sender], [e])");
			FriendList.Invalidate(true);
			_lastPressStartY = -1;
			_log.Debug("<< FriendListScroll()");
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

		public void HandleGesture(object sender, GestureEventArgs e)
		{
			// Do not log gestures, it gets VERY SPAMMY

			if (_handlingGesture || e.Gesture != RazerAPI.RZGESTURE.PRESS)
				return;

			//_log.Debug(">> HandleGesture([e])");
			var param = e.Parameter;

			//_log.DebugFormat("param == {0}", param);

			int y = e.Y;

			if (param == 1) // Active
			{
				if (_lastY == -1) // This is the first press event
				{
					_lastY = y;
					_yChange = 0;
				}

				if (_lastPressParam == 0)
					_lastPressStartY = y;

				_yChange += (_lastY - y);

				if (_yChange >= _yScrollDetectAmount) // Scroll up
				{
					_log.DebugFormat("_yChange == {0} - Scrolling up", _yChange);

					try
					{
						WinAPI.SendMessage(FriendList.Handle, WinAPI.WM_VSCROLL, (IntPtr) ScrollEventType.SmallIncrement, IntPtr.Zero);
					}
					catch (ObjectDisposedException)
					{
						_log.Warn("Scroll action failed because ListView is disposed, aborting...");
					}
					finally
					{
						_yChange = 0;
					}
				}
				else if (_yChange <= -_yScrollDetectAmount) // Scroll down
				{
					_log.DebugFormat("_yChange == {0} - Scrolling down", _yChange);

					try
					{
						WinAPI.SendMessage(FriendList.Handle, WinAPI.WM_VSCROLL, (IntPtr) ScrollEventType.SmallDecrement, IntPtr.Zero);
					}
					catch (ObjectDisposedException)
					{
						_log.Warn("Scroll action failed because ListView is disposed, aborting...");
					}
					finally
					{
						_yChange = 0;	
					}
				}
			}
			else if (param == 0) // End of press
			{
				_log.Debug("End of press");

				if (_lastPressStartY >= 0)
				{
					var lowerLimit = _lastPressStartY - (_yScrollDetectAmount / 2);
					var upperLimit = _lastPressStartY + (_yScrollDetectAmount / 2);

					if (y >= lowerLimit && y <= upperLimit) // Friend pressed
					{
						_log.Debug("Friend pressed!");
						var selected = FriendList.GetItemAt(e.X, y);
						if (selected != null)
						{
							_log.Debug("Valid selection, starting chat...");
							StartChat((CSteamID)selected.Tag);
						}
						else
							_log.Debug("Not a valid selection");
					}
				}

				_yChange = 0;
			}

			_lastY = y;
			_lastPressParam = (int) param;

			_handlingGesture = false;
			//_log.Debug("<< HandleGesture()");
		}
	}
}
