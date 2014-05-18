using System;
using System.Collections.Generic;
using System.Windows.Threading;
using Sharparam.SteamLib;
using Sharparam.SwitchBladeSteam.Lib;

using SharpBlade.Native;
using SharpBlade.Razer;

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

        private static readonly Dictionary<EPersonaState, string[]> ImageMap = new Dictionary<EPersonaState, string[]>
        {
            {
                EPersonaState.k_EPersonaStateOnline,
                new[]
                {
                    @"Default\Images\dk_online_alt.png",
                    @"Default\Images\dk_online_pressed_alt.png",
                    @"Default\Images\dk_online_active_alt.png"
                }
            },
            {
                EPersonaState.k_EPersonaStateBusy,
                new[]
                {
                    @"Default\Images\dk_busy_alt.png",
                    @"Default\Images\dk_busy_pressed_alt.png",
                    @"Default\Images\dk_busy_active_alt.png"
                }
            },
            {
                EPersonaState.k_EPersonaStateAway,
                new[]
                {
                    @"Default\Images\dk_away_alt.png",
                    @"Default\Images\dk_away_pressed_alt.png",
                    @"Default\Images\dk_away_active_alt.png"
                }
            },
            {
                EPersonaState.k_EPersonaStateOffline,
                new[]
                {
                    @"Default\Images\dk_offline_alt.png",
                    @"Default\Images\dk_offline_pressed_alt.png",
                    @"Default\Images\dk_offline_active_alt.png"
                }
            }
        };

        private readonly Dictionary<EPersonaState, DynamicKey> _dynamicKeys; 

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

            _dynamicKeys = new Dictionary<EPersonaState, DynamicKey>();

            _dynamicKeys[EPersonaState.k_EPersonaStateOnline] = razer.EnableDynamicKey(
                RazerAPI.DynamicKeyType.DK6,
                OnOnlineKeyPressed,
                ImageMap[EPersonaState.k_EPersonaStateOnline][0],
                ImageMap[EPersonaState.k_EPersonaStateOnline][1],
                true);

            _dynamicKeys[EPersonaState.k_EPersonaStateBusy] = razer.EnableDynamicKey(
                RazerAPI.DynamicKeyType.DK7,
                OnBusyKeyPressed,
                ImageMap[EPersonaState.k_EPersonaStateBusy][0],
                ImageMap[EPersonaState.k_EPersonaStateBusy][1],
                true);

            _dynamicKeys[EPersonaState.k_EPersonaStateAway] = razer.EnableDynamicKey(
                RazerAPI.DynamicKeyType.DK8,
                OnAwayKeyPressed,
                ImageMap[EPersonaState.k_EPersonaStateAway][0],
                ImageMap[EPersonaState.k_EPersonaStateAway][1],
                true);

            _dynamicKeys[EPersonaState.k_EPersonaStateOffline] = razer.EnableDynamicKey(
                RazerAPI.DynamicKeyType.DK9,
                OnOfflineKeyPressed,
                ImageMap[EPersonaState.k_EPersonaStateOffline][0],
                ImageMap[EPersonaState.k_EPersonaStateOffline][1],
                true);

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

        private static EPersonaState NormalizeState(EPersonaState state)
        {
            return StateMap.ContainsKey(state) ? StateMap[state] : state;
        }

        private static bool EqualStates(EPersonaState first, EPersonaState second)
        {
            return NormalizeState(first) == NormalizeState(second);
        }

        private void ResetKey(EPersonaState state)
        {
            state = NormalizeState(state);

            _dynamicKeys[state].SetImages(ImageMap[state][0], ImageMap[state][1]);
        }

        private void UpdateKeys(EPersonaState state, bool ignoreState = false)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.Invoke(DispatcherPriority.Render, (VoidDelegate) (() => UpdateKeys(state, ignoreState)));
                return;
            }

            // _state is old state, state is new state

            // Do nothing if the state didn't change
            if (!ignoreState && EqualStates(state, _state)) // state == _state
                return;

            // Reset the previous key to the default images
            // EX: _state = online, state = away, reset online key to default image, set away to active image
            ResetKey(_state);

            var normalizedState = NormalizeState(state);

            _dynamicKeys[normalizedState].SetImage(ImageMap[normalizedState][2]);

            _state = state;
        }

        private void LocalUserOnStateChanged(object sender, EventArgs eventArgs)
        {
            UpdateKeys(Provider.Steam.LocalUser.State);
        }
    }
}
