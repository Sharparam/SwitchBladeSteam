using Sharparam.SharpBlade.Razer;
using Sharparam.SteamLib;

namespace Sharparam.SwitchBladeSteam.Lib
{
    public static class Provider
    {
        private static RazerManager _razer;
        private static Steam _steam;

        public static RazerManager Razer { get { return _razer ?? (_razer = new RazerManager()); } }
        public static Steam Steam { get { return _steam ?? (_steam = new Steam()); } }
    }
}
