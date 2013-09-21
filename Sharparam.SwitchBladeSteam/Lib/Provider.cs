using System.Windows;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer;
using Sharparam.SharpBlade.Razer.Events;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.ViewModels;

namespace Sharparam.SwitchBladeSteam.Lib
{
    public static class Provider
    {
        private static RazerManager _razer;
        private static Steam _steam;

        public static RazerManager Razer
        {
            get
            {
                if (_razer == null)
                {
                    _razer = new RazerManager();
                    _razer.AppEvent += OnAppEvent;
                }

                return _razer;
            }
        }

        public static Steam Steam { get { return _steam ?? (_steam = new Steam()); } }

        private static void OnAppEvent(object sender, AppEventEventArgs e)
        {
            switch (e.Type)
            {
                case RazerAPI.AppEventType.Close:
                case RazerAPI.AppEventType.Deactivated:
                case RazerAPI.AppEventType.Exit:
                    FriendViewModel.ClearCache();
                    if (_steam != null)
                        _steam.Dispose();
                    Application.Current.Shutdown();
                    break;
            }
        }
    }
}
