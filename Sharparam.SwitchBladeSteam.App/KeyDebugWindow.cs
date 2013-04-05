/* KeyDebugWindow.cs
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
using System.Windows.Forms;
using Sharparam.SwitchBladeSteam.App.Properties;

namespace Sharparam.SwitchBladeSteam.App
{
    public partial class KeyDebugWindow : Form
    {
        public KeyDebugWindow()
        {
            InitializeComponent();
            HomeButton.MouseDown += HomeButtonMouseDown;
            HomeButton.MouseUp += HomeButtonMouseUp;
            FriendsButton.MouseDown += FriendsButtonMouseDown;
            FriendsButton.MouseUp += FriendsButtonMouseUp;
            OnlineButton.MouseDown += OnlineButtonMouseDown;
            OnlineButton.MouseUp += OnlineButtonMouseUp;
            OfflineButton.MouseDown += OfflineButtonMouseDown;
            OfflineButton.MouseUp += OfflineButtonMouseUp;
            UpButton.MouseDown += UpButtonMouseDown;
            UpButton.MouseUp += UpButtonMouseUp;
            DownButton.MouseDown += DownButtonMouseDown;
            DownButton.MouseUp += DownButtonMouseUp;
            ChatButton.MouseDown += ChatButtonMouseDown;
            ChatButton.MouseUp += ChatButtonMouseUp;
        }

        private void HomeButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugHomeButton();
#endif
        }

        private void HomeButtonMouseDown(object sender, EventArgs e)
        {
            HomeButton.Image = Resources.dk_home_down;
        }

        private void HomeButtonMouseUp(object sender, EventArgs e)
        {
            HomeButton.Image = Resources.dk_home;
        }

        private void FriendsButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugFriendsButton();
#endif
        }

        private void FriendsButtonMouseDown(object sender, EventArgs e)
        {
            FriendsButton.Image = Resources.dk_friends_down;
        }

        private void FriendsButtonMouseUp(object sender, EventArgs e)
        {
            FriendsButton.Image = Resources.dk_friends;
        }

        private void OnlineButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugOnlineButton();
#endif
        }

        private void OnlineButtonMouseDown(object sender, EventArgs e)
        {
            OnlineButton.Image = Resources.dk_appear_online_down;
        }

        private void OnlineButtonMouseUp(object sender, EventArgs e)
        {
            OnlineButton.Image = Resources.dk_appear_online;
        }

        private void OfflineButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugOfflineButton();
#endif
        }

        private void OfflineButtonMouseDown(object sender, EventArgs e)
        {
            OfflineButton.Image = Resources.dk_appear_offline_down;
        }

        private void OfflineButtonMouseUp(object sender, EventArgs e)
        {
            OfflineButton.Image = Resources.dk_appear_offline;
        }

        private void UpButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugUpButton();
#endif
        }

        private void UpButtonMouseDown(object sender, EventArgs e)
        {
            UpButton.Image = Resources.dk_up_down;
        }

        private void UpButtonMouseUp(object sender, EventArgs e)
        {
            UpButton.Image = Resources.dk_up;
        }

        private void DownButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugDownButton();
#endif
        }

        private void DownButtonMouseDown(object sender, EventArgs e)
        {
            DownButton.Image = Resources.dk_down_down;
        }

        private void DownButtonMouseUp(object sender, EventArgs e)
        {
            DownButton.Image = Resources.dk_down;
        }

        private void ChatButtonClick(object sender, EventArgs e)
        {
#if DEBUG
            Program.DebugChatButton();
#endif
        }

        private void ChatButtonMouseDown(object sender, EventArgs e)
        {
            ChatButton.Image = Resources.dk_chat_down;
        }

        private void ChatButtonMouseUp(object sender, EventArgs e)
        {
            ChatButton.Image = Resources.dk_chat;
        }
    }
}
