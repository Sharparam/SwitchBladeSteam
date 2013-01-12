/* ChatMessage.cs
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

using System.Text;
using F16Gaming.SwitchBladeSteam.Steam.Events;
using Steam4NET;

namespace F16Gaming.SwitchBladeSteam.Steam
{
	public class Client
	{
		// Events
		public event ChatMessageReceivedEventHandler ChatMessageReceived;

		// Interfaces
		private ISteamClient012 _steamClient;
		private IClientEngine _clientEngine;
		private IClientFriends _clientFriends;
		private ISteamUser016 _steamUser;
		private ISteamUtils005 _steamUtils;

		private FriendsManager _friendsManager;

		private int _pipe;
		private int _user;

		// Callbacks
		private Callback<FriendChatMsg_t> _friendChatCallback;
		private Callback<FriendAdded_t> _friendAddedCallback;
		private Callback<PersonaStateChange_t> _personaStateChangeCallback;
		private Callback<FriendProfileInfoResponse_t> _friendProfileInfoResponseCallback; 

		private bool _running;

		// Accessors
		public CSteamID Me { get { return _steamUser.GetSteamID(); } }
		public FriendsManager FriendsManager { get { return _friendsManager; } }

		public Client()
		{
			if (!Steamworks.Load())
				throw new SteamException("Steamworks failed to load");
			
			_steamClient = Steamworks.CreateInterface<ISteamClient012>();

			if (_steamClient == null)
				throw new SteamException("SteamClient is null");

			GetSteamPipe();
			GetSteamUser();

			_steamUser = _steamClient.GetISteamUser<ISteamUser016>(_user, _pipe);

			if (_steamUser == null)
				throw new SteamException("SteamUser is null!");

			_steamUtils = _steamClient.GetISteamUtils<ISteamUtils005>(_pipe);

			_clientEngine = Steamworks.CreateInterface<IClientEngine>();

			if (_clientEngine == null)
				throw new SteamException("ClientEngine is null");

			_clientFriends = _clientEngine.GetIClientFriends<IClientFriends>(_user, _pipe);

			if (_clientFriends == null)
				throw new SteamException("ClientFriends is null");

			_friendsManager = new FriendsManager(_clientFriends, _steamUtils);
			
			// Set up callbacks
			_friendChatCallback = new Callback<FriendChatMsg_t>(HandleFriendChatMessage);
			_friendAddedCallback = new Callback<FriendAdded_t>(HandleFriendAdded);
			_personaStateChangeCallback = new Callback<PersonaStateChange_t>(HandlePersonaStateChange);
			_friendProfileInfoResponseCallback = new Callback<FriendProfileInfoResponse_t>(HandleFriendProfileInfoResponse);

			CallbackDispatcher.SpawnDispatchThread(_pipe);

			_running = true;
		}

		private void GetSteamPipe()
		{
			if (_pipe != 0)
				_steamClient.BReleaseSteamPipe(_pipe);
			_pipe = _steamClient.CreateSteamPipe();
			if (_pipe == 0)
				throw new SteamException("Unable to create steam pipe (_pipe == 0)!");
		}

		private void GetSteamUser()
		{
			if (_user != 0)
				_steamClient.ReleaseUser(_pipe, _user);
			_user = _steamClient.ConnectToGlobalUser(_pipe);
			if (_user == 0)
				throw new SteamException("Unable to connect to global user (_user == 0)!");
		}

		private void OnChatMessageReceived(ChatMessage message)
		{
			var func = ChatMessageReceived;
			if (func == null)
				return;
			func(this, new ChatMessageEventArgs(message));
		}

		public bool IsMe(CSteamID id)
		{
			return id == Me;
		}

		public string GetMyName()
		{
			return _clientFriends.GetPersonaName();
		}

		private void HandleFriendChatMessage(FriendChatMsg_t callback)
		{
			// Messages should usually be only between Me -> Friend or Friend -> Me
			// Unknown if this catches group chats, probably not
			var sender = new CSteamID(callback.m_ulSenderID); // Who sent the message
			var receiver = new CSteamID(callback.m_ulFriendID); // Who received the message
			var bytes = new byte[4096]; // Create byte array to hold message data
			var type = EChatEntryType.k_EChatEntryTypeChatMsg; // Set a temporary message type
			var length = _clientFriends.GetChatMessage(receiver, (int) callback.m_iChatID, bytes, ref type, ref sender); // Load message into bytes, also get type, sender and length of message
			if (type != EChatEntryType.k_EChatEntryTypeChatMsg && type != EChatEntryType.k_EChatEntryTypeEmote) // We only care about normal or emote messages
				return;
			
			string msg = Encoding.UTF8.GetString(bytes, 0, length - 1); // Convert the byte array to string
			var message = new ChatMessage(sender, receiver, type, msg); // Construct the message class
			OnChatMessageReceived(message); // Throw the message event
		}

		private void HandleFriendAdded(FriendAdded_t callback)
		{
			_friendsManager.UpdateFriends(); // A friend was added, so we need to update the local friends list
		}

		private void HandlePersonaStateChange(PersonaStateChange_t callback)
		{
			// TODO: Just refresh the entire friends list for now, make it more efficient later
			var changeType = callback.m_nChangeFlags;
			if (changeType == EPersonaChange.k_EPersonaChangeComeOnline ||
			    changeType == EPersonaChange.k_EPersonaChangeGoneOffline ||
			    changeType == EPersonaChange.k_EPersonaChangeName ||
			    changeType == EPersonaChange.k_EPersonaChangeNameFirstSet ||
			    changeType == EPersonaChange.k_EPersonaChangeRelationshipChanged ||
			    changeType == EPersonaChange.k_EPersonaChangeStatus)
			{
				_friendsManager.UpdateFriends();
			}
		}

		private void HandleFriendProfileInfoResponse(FriendProfileInfoResponse_t callback)
		{
			// TODO: Just refresh the entire friends list for now, make it more efficient later
			_friendsManager.UpdateFriends();
		}
	}
}
