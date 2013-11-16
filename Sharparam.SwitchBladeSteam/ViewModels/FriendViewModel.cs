using System.Collections.Generic;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Compatibility;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class FriendViewModel
    {
        private static readonly Dictionary<Friend, FriendViewModel> Cache = new Dictionary<Friend, FriendViewModel>();

        public Friend Friend { get; private set; }

        public MessagesWrapper Messages { get; private set; }

        private FriendViewModel(Friend friend)
        {
            Friend = friend;
            
            Messages = new MessagesWrapper(Friend.MessageHistory);
        }

        public static FriendViewModel GetViewModel(Friend friend)
        {
            if (Cache.ContainsKey(friend))
                return Cache[friend];

            var viewModel = new FriendViewModel(friend);
            Cache.Add(friend, viewModel);
            return viewModel;
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
