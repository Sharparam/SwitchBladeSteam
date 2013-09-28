﻿using System;
using Sharparam.SharpBlade.Native;
using Sharparam.SharpBlade.Razer;
using Sharparam.SwitchBladeSteam.Lib;
using Steam4NET;

namespace Sharparam.SwitchBladeSteam
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly DynamicKey _onlineKey;
        private readonly DynamicKey _busyKey;
        private readonly DynamicKey _awayKey;
        private readonly DynamicKey _offlineKey;

        private EPersonaState _state;

        public App()
        {
            var razer = Provider.Razer;

            _onlineKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK6, OnOnlineKeyPressed, @"Resources\Images\dk_online.png",
                                                @"Resources\Images\dk_online_down.png", true);
            _busyKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK7, OnBusyKeyPressed, @"Resources\Images\dk_busy.png",
                                              @"Resources\Images\dk_busy_down.png", true);
            _awayKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK8, OnAwayKeyPressed,
                                              @"Resources\Images\dk_away.png",
                                              @"Resources\Images\dk_away_down.png", true);
            _offlineKey = razer.EnableDynamicKey(RazerAPI.DynamicKeyType.DK9, OnOfflineKeyPressed,
                                                 @"Resources\Images\dk_offline.png",
                                                 @"Resources\Images\dk_offline_down.png", true);
            
            Provider.Steam.LocalUser.StateChanged += LocalUserOnStateChanged;
            UpdateKeys(Provider.Steam.LocalUser.State, true);
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

        private void ResetKeys()
        {
            _onlineKey.SetImages(@"Resources\Images\dk_online.png", @"Resources\Images\dk_online_down.png");
            _busyKey.SetImages(@"Resources\Images\dk_busy.png", @"Resources\Images\dk_busy_down.png");
            _awayKey.SetImages(@"Resources\Images\dk_away.png", @"Resources\Images\dk_away_down.png");
            _offlineKey.SetImages(@"Resources\Images\dk_offline.png", @"Resources\Images\dk_offline_down.png");
        }

        private void UpdateKeys(EPersonaState state, bool ignoreState = false)
        {
            if (!ignoreState && state == _state)
                return;

            ResetKeys();

            switch (state)
            {
                case EPersonaState.k_EPersonaStateOnline:
                case EPersonaState.k_EPersonaStateMax:
                case EPersonaState.k_EPersonaStateLookingToPlay:
                case EPersonaState.k_EPersonaStateLookingToTrade:
                    _onlineKey.SetImage(@"Resources\Images\dk_online_active.png");
                    break;
                case EPersonaState.k_EPersonaStateBusy:
                    _busyKey.SetImage(@"Resources\Images\dk_busy_active.png");
                    break;
                case EPersonaState.k_EPersonaStateAway:
                case EPersonaState.k_EPersonaStateSnooze:
                    _awayKey.SetImage(@"Resources\Images\dk_away_active.png");
                    break;
                case EPersonaState.k_EPersonaStateOffline:
                    _offlineKey.SetImage(@"Resources\Images\dk_offline_active.png");
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
