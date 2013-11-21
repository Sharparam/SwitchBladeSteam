using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private delegate void VoidDelegate();

        private static readonly Dictionary<EPersonaState, EPersonaState> StateMap = new Dictionary<EPersonaState, EPersonaState>
        {
            {EPersonaState.k_EPersonaStateLookingToPlay, EPersonaState.k_EPersonaStateOnline},
            {EPersonaState.k_EPersonaStateLookingToTrade, EPersonaState.k_EPersonaStateOnline},
            {EPersonaState.k_EPersonaStateMax, EPersonaState.k_EPersonaStateOnline},
            {EPersonaState.k_EPersonaStateSnooze, EPersonaState.k_EPersonaStateAway}
        };

        private readonly Dispatcher _dispatcher;

        private readonly DynamicKey _onlineKey;
        private readonly DynamicKey _busyKey;
        private readonly DynamicKey _awayKey;
        private readonly DynamicKey _offlineKey;

        private EPersonaState _state;

        public App()
        {
            _dispatcher = Current.Dispatcher;

            var razer = Provider.Razer;

            _onlineKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK6, OnOnlineKeyPressed, @"Default\Images\dk_online.png",
                                                @"Default\Images\dk_online_pressed.png", true);
            _busyKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK7, OnBusyKeyPressed, @"Default\Images\dk_busy.png",
                                              @"Deault\Images\dk_busy_pressed.png", true);
            _awayKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK8, OnAwayKeyPressed,
                                              @"Default\Images\dk_away.png",
                                              @"Default\Images\dk_away_pressed.png", true);
            _offlineKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK9, OnOfflineKeyPressed,
                                                 @"Default\Images\dk_offline.png",
                                                 @"Default\Images\dk_offline_pressed.png", true);

            //razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK2, (sender, args) =>
            //    {
            //        _onlineKey.Refresh();
            //        _busyKey.Refresh();
            //        _awayKey.Refresh();
            //        _offlineKey.Refresh();
            //    }, "null");

            try
            {
                var steam = Provider.Steam;
                steam.LocalUser.StateChanged += LocalUserOnStateChanged;
                UpdateKeys(steam.LocalUser.State, true);
            }
            catch (SteamException)
            {
                razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK6);
                razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK7);
                razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK8);
                razer.DisableDynamicKey(RazerAPI.DynamicKeyType.DK9);
            }
        }

        private void OnOnlineKeyPressed(object sender, EventArgs eventArgs)
        {
            Provider.Steam.LocalUser.State = EPersonaState.k_EPersonaStateOnline;
        }

        private static void OnBusyKeyPressed(object sender, EventArgs eventArgs)
        {
            Provider.Steam.LocalUser.State = EPersonaState.k_EPersonaStateBusy;
        }

        private static void OnAwayKeyPressed(object sender, EventArgs eventArgs)
        {
            Provider.Steam.LocalUser.State = EPersonaState.k_EPersonaStateAway;
        }

        private static void OnOfflineKeyPressed(object sender, EventArgs eventArgs)
        {
            Provider.Steam.LocalUser.State = EPersonaState.k_EPersonaStateOffline;
        }

        private static bool EqualStates(EPersonaState first, EPersonaState second)
        {
            if (StateMap.ContainsKey(first))
                first = StateMap[first];

            if (StateMap.ContainsKey(second))
                second = StateMap[second];

            return first == second;
        }

        private void ResetKey(EPersonaState state)
        {
            switch (state)
            {
                case EPersonaState.k_EPersonaStateOnline:
                case EPersonaState.k_EPersonaStateMax:
                case EPersonaState.k_EPersonaStateLookingToPlay:
                case EPersonaState.k_EPersonaStateLookingToTrade:
                    Console.WriteLine("Resetting online key");
                    _onlineKey.SetImages(@"Default\Images\dk_online.png", @"Default\Images\dk_online_pressed.png");
                    break;
                case EPersonaState.k_EPersonaStateBusy:
                    Console.WriteLine("Resetting busy key");
                    _busyKey.SetImages(@"Default\Images\dk_busy.png", @"Default\Images\dk_busy_pressed.png");
                    break;
                case EPersonaState.k_EPersonaStateAway:
                case EPersonaState.k_EPersonaStateSnooze:
                    Console.WriteLine("Resetting away key");
                    _awayKey.SetImages(@"Default\Images\dk_away.png", @"Default\Images\dk_away_pressed.png");
                    break;
                case EPersonaState.k_EPersonaStateOffline:
                    Console.WriteLine("Resetting offline key");
                    _offlineKey.SetImages(@"Default\Images\dk_offline.png", @"Default\Images\dk_offline_pressed.png");
                    break;
            }
        }

        private void UpdateKeys(EPersonaState state, bool ignoreState = false)
        {
            if (!_dispatcher.CheckAccess())
            {
                Console.WriteLine("UpdateKeys called from thread that is not main, invoking via dispatcher...");
                _dispatcher.Invoke(DispatcherPriority.Render, (VoidDelegate) (() => UpdateKeys(state, ignoreState)));
            }

            // _state is old state, state is new state

            // Do nothing if the state didn't change
            if (!ignoreState && (EqualStates(state, _state) || EqualStates(_state, state))) // state == _state
                return;

            // Reset the previous key to the default images
            // EX: _state = online, state = away, reset online key to default image, set away to active image
            ResetKey(_state);

            switch (state)
            {
                case EPersonaState.k_EPersonaStateOnline:
                case EPersonaState.k_EPersonaStateMax:
                case EPersonaState.k_EPersonaStateLookingToPlay:
                case EPersonaState.k_EPersonaStateLookingToTrade:
                    Console.WriteLine("Activating online key");
                    _onlineKey.SetImage(@"Default\Images\dk_online_active.png");
                    break;
                case EPersonaState.k_EPersonaStateBusy:
                    Console.WriteLine("Activating busy key");
                    _busyKey.SetImage(@"Default\Images\dk_busy_active.png");
                    break;
                case EPersonaState.k_EPersonaStateAway:
                case EPersonaState.k_EPersonaStateSnooze:
                    Console.WriteLine("Activating away key");
                    _awayKey.SetImage(@"Default\Images\dk_away_active.png");
                    break;
                case EPersonaState.k_EPersonaStateOffline:
                    Console.WriteLine("Activating offline key");
                    _offlineKey.SetImage(@"Default\Images\dk_offline_active.png");
                    break;
            }

            _state = state;
        }

        private void LocalUserOnStateChanged(object sender, EventArgs eventArgs)
        {
            UpdateKeys(Provider.Steam.LocalUser.State);
        }
    }
}
