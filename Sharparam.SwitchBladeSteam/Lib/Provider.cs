using Sharparam.SharpBlade.Razer;
using Sharparam.SteamLib;

namespace Sharparam.SwitchBladeSteam.Lib
{
    public static class Provider
    {
        private static RazerManager _razer;
        private static Client _steam;

        public static RazerManager Razer { get { return _razer ?? (_razer = new RazerManager()); } }
        public static Client Steam { get { return _steam ?? (_steam = new Client()); } }
    }
}
