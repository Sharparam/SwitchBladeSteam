/* Friend.cs
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
using System.Text;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.Steam
{
	public class Friend
	{
		private log4net.ILog _log;

		private IClientFriends _clientFriends;
		private CSteamID _steamId;

		public CSteamID SteamID { get { return _steamId; } }
		public Bitmap Avatar { get; internal set; }

		public bool Online { get { return GetState() != EPersonaState.k_EPersonaStateOffline; } }

		internal Friend(IClientFriends clientFriends, CSteamID id)
		{
			_log = Logging.LogManager.GetLogger(this);
			_log.DebugFormat(">> Friend([clientFriends], {0})", id.Render());
			_clientFriends = clientFriends;
			_steamId = id;
			_log.Debug("<< Friend()");
		}

		public string GetName()
		{
			_log.Debug(">< GetName()");
			return _clientFriends.GetFriendPersonaName(_steamId);
		}

		public string GetNickname()
		{
			_log.Debug(">> GetNickName()");
			string nick;
			try
			{
				nick = _clientFriends.GetPlayerNickname(_steamId);
			}
			catch (ArgumentNullException) // No nickname set for this friend
			{
				nick = null;
			}
			_log.Debug("<< GetNickName()");
			return nick;
		}

		public EPersonaState GetState()
		{
			_log.Debug(">< GetState()");
			return _clientFriends.GetFriendPersonaState(_steamId);
		}

		public string GetStateText()
		{
			_log.Debug(">< GetStateText()");
			return Utils.StateToString(GetState());
		}

		public void SendType(string message, EChatEntryType type)
		{
			_log.DebugFormat(">> SendType([message], {0})", type);
			Console.WriteLine("Sending to {0} to {1}: {2}", type, _steamId.Render(), message);
			_clientFriends.SendMsgToFriend(_steamId, type, Encoding.UTF8.GetBytes(message));
			_log.Debug("<< SendType()");
		}

		public void SendMessage(string message)
		{
			_log.Debug(">> SendMessage([message])");
			SendType(message, EChatEntryType.k_EChatEntryTypeChatMsg);
			_log.Debug("<< SendMessage()");
		}

		public void SendEmote(string message)
		{
			_log.Debug(">> SendEmote([message])");
			SendType(message, EChatEntryType.k_EChatEntryTypeEmote);
			_log.Debug("<< SendEmote()");
		}
	}
}
