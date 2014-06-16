using System.ComponentModel;
using System.Windows;

using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;

namespace Sharparam.SwitchBladeSteam.ViewModels
{
    public class GamesViewModel
    {
        public Apps Apps { get; private set; }

        public GamesViewModel() 
        {
            var dep = new DependencyObject();
            if (DesignerProperties.GetIsInDesignMode(dep))
                return;

            Apps = Provider.Steam.Apps;
        }
    }
}
