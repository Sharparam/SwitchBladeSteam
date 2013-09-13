using System.ComponentModel;
using System.Windows;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class FriendsViewModel
    {
        public FriendsList Friends { get; private set; }

        public FriendsViewModel()
        {
            var dep = new DependencyObject();
            if (DesignerProperties.GetIsInDesignMode(dep))
                return;

            Friends = Provider.Steam.Friends;
        }
    }
}
