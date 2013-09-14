using System.ComponentModel;
using System.Windows;
using Sharparam.SwitchBladeSteam.Compatibility;
using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class FriendsViewModel
    {
        public FriendsWrapper Friends { get; private set; }

        public FriendsViewModel()
        {
            var dep = new DependencyObject();
            if (DesignerProperties.GetIsInDesignMode(dep))
                return;

            Friends = new FriendsWrapper(Provider.Steam.Friends);
        }
    }
}
