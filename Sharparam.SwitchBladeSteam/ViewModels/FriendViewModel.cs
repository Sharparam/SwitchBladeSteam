using Sharparam.SteamLib;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class FriendViewModel
    {
        public Friend Friend { get; private set; }

        public FriendViewModel(Friend friend)
        {
            Friend = friend;
        }
    }
}
