/* ChatWindow.cs
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
using System.Windows.Forms;
using F16Gaming.SwitchBladeSteam.Razer;
using F16Gaming.SwitchBladeSteam.Steam;
using F16Gaming.SwitchBladeSteam.Steam.Events;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.App
{
	public partial class ChatWindow : Form
	{
		private const string ChatLineFormat = "{0}: {1}";

		private Friend _friend;

		public ChatWindow(CSteamID friendId)
		{
			InitializeComponent();

			_friend = Program.SteamFriends.GetFriendBySteamId(friendId);
			ChatTitle.Text = "Chatting with " + _friend.GetName();
			Program.SteamClient.ChatMessageReceived += HandleChatMessage;
		}

		private void EntryBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Enter || string.IsNullOrEmpty(EntryBox.Text))
				return;

			var message = EntryBox.Text;
			_friend.SendMessage(message + "\0");
			EntryBox.Clear();
		}

		private void EntryBox_TextChanged(object sender, EventArgs e)
		{
			EntryBox.Text = EntryBox.Text.TrimStart('\n', '\r', ' ');
		}

		private void WriteChatMessage(string name, string message)
		{
			ChatHistory.SelectionColor = Color.CornflowerBlue;
			ChatHistory.AppendText(name + ": ");
			ChatHistory.SelectionColor = Color.DarkGray;
			ChatHistory.AppendText(message + Environment.NewLine);
		}

		private void HandleChatMessage(object sender, ChatMessageEventArgs e)
		{
			var msg = e.Message;
			string senderName;
			if (Program.SteamClient.IsMe(msg.Sender))
				senderName = Program.SteamClient.GetMyName();
			else
				senderName = Program.SteamFriends.GetFriendBySteamId(msg.Sender).GetName();
			var message = msg.Message;
			if (ChatHistory.InvokeRequired)
				ChatHistory.Invoke((VoidDelegate) (() => WriteChatMessage(senderName, message)));
			else
				WriteChatMessage(senderName, message);
		}

		private void ChatWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Program.SteamClient.ChatMessageReceived -= HandleChatMessage;
		}
	}
}
