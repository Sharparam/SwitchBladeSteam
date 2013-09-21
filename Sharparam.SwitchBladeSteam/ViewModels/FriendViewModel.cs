using System.Collections.Generic;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Compatibility;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class FriendViewModel
    {
        private static readonly Dictionary<Friend, FriendViewModel> _cache = new Dictionary<Friend, FriendViewModel>();

        public Friend Friend { get; private set; }

        public MessagesWrapper Messages { get; private set; }

        private FriendViewModel(Friend friend)
        {
            Friend = friend;
            
            Messages = new MessagesWrapper(Friend.MessageHistory);
        }

        public static FriendViewModel GetViewModel(Friend friend)
        {
            if (_cache.ContainsKey(friend))
                return _cache[friend];

            var viewModel = new FriendViewModel(friend);
            _cache.Add(friend, viewModel);
            return viewModel;
        }

        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}
